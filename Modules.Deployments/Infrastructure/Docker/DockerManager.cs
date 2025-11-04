using Docker.DotNet;
using Modules.Deployments.Application.Interfaces;
using Docker.DotNet.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using Modules.Logs.Application.Interfaces;

namespace Modules.Deployments.Infrastructure.Docker
{
    public class DockerManager : IDockerManager
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerManager> _logger;
        private readonly IDeploymentLogService _deploymentLogService;
        private readonly IRuntimeDetector _runtimeDetector;

        public DockerManager(
            ILogger<DockerManager> logger,
            IDeploymentLogService deploymentLogService,
            IRuntimeDetector runtimeDetector)
        {
            _logger = logger;
            _deploymentLogService = deploymentLogService;
            _runtimeDetector = runtimeDetector;

            _dockerClient = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine")
            ).CreateClient();
        }

        public async Task<string> BuildAndRunContainerAsync(string repoPath, int port, CancellationToken cancellationToken)
        {
            var appInstanceId = Guid.NewGuid(); // Unique ID per deployment

            try
            {
                // 🧩 STEP 1: Prepare Dockerfile and .dockerignore
                _runtimeDetector.EnsureDockerfileExists(repoPath);
                _runtimeDetector.EnsureDockerignoreExists(repoPath);

                var prepMsg = $"Verified Dockerfile and .dockerignore exist for repo {repoPath}";
                _logger.LogInformation(prepMsg);
                await _deploymentLogService.AppendLogAsync(appInstanceId, prepMsg, cancellationToken);

                // 🧱 STEP 2: Create TAR and build image
                string imageTag = $"app_{appInstanceId}:latest";

                using (var buildContext = BuildContextFromDirectory(repoPath))
                {
                    var buildParams = new ImageBuildParameters
                    {
                        Tags = new[] { imageTag },
                    };

                    var progress = new Progress<JSONMessage>(async message =>
                    {
                        if (!string.IsNullOrEmpty(message.Stream))
                        {
                            var log = $"[Docker] {message.Stream.Trim()}";
                            _logger.LogInformation(log);
                            await _deploymentLogService.AppendLogAsync(appInstanceId, log, cancellationToken);
                        }

                        if (!string.IsNullOrEmpty(message.ErrorMessage))
                        {
                            var err = $"[Docker Error] {message.ErrorMessage}";
                            _logger.LogError(err);
                            await _deploymentLogService.AppendLogAsync(appInstanceId, err, cancellationToken);
                        }
                    });

                    var startMsg = $"Starting Docker image build for {repoPath}";
                    _logger.LogInformation(startMsg);
                    await _deploymentLogService.AppendLogAsync(appInstanceId, startMsg, cancellationToken);

                    await _dockerClient.Images.BuildImageFromDockerfileAsync(
                        buildParams,
                        buildContext,
                        null,
                        null,
                        progress,
                        cancellationToken
                    );

                    await Task.Delay(500, cancellationToken);

                    var images = await _dockerClient.Images.ListImagesAsync(
                        new ImagesListParameters
                        {
                            Filters = new Dictionary<string, IDictionary<string, bool>>
                        { { "reference", new Dictionary<string, bool> { { imageTag, true } } } }
                        },
                        cancellationToken
                    );

                    if (images == null || images.Count == 0)
                    {
                        var failedMsg = $"Image not found after build: {imageTag}";
                        _logger.LogInformation(failedMsg);
                        throw new InvalidOperationException($"Image not found after build: {imageTag}");
                    }

                    var successMsg = "✅ Image built successfully.";
                    _logger.LogInformation(successMsg);
                    await _deploymentLogService.AppendLogAsync(appInstanceId, successMsg, cancellationToken);
                }

                // ⚙️ STEP 3: Create and run container
                var createMsg = $"Creating container on port {port}...";
                _logger.LogInformation(createMsg);
                await _deploymentLogService.AppendLogAsync(appInstanceId, createMsg, cancellationToken);

                var createResponse = await _dockerClient.Containers.CreateContainerAsync(new CreateContainerParameters
                {
                    Image = imageTag,
                    ExposedPorts = new Dictionary<string, EmptyStruct> {
                        { port.ToString(), default }
                    },
                    HostConfig = new HostConfig
                    {
                        PortBindings = new Dictionary<string, IList<PortBinding>>
                        {
                            [port.ToString()] = new List<PortBinding> { new() { HostPort = port.ToString() } }
                        }
                    }
                }, cancellationToken);

                await _dockerClient.Containers.StartContainerAsync(createResponse.ID, new ContainerStartParameters(), cancellationToken);

                var containerMsg = $"🚀 Container started successfully. ID: {createResponse.ID}";
                _logger.LogInformation(containerMsg);
                await _deploymentLogService.AppendLogAsync(appInstanceId, containerMsg, cancellationToken);

                return createResponse.ID;
            }
            catch (Exception ex)
            {
                var errMsg = $"❌ Error during Docker build or container start for repo {repoPath}: {ex.Message}";
                _logger.LogError(ex, errMsg);
                await _deploymentLogService.AppendLogAsync(Guid.Empty, errMsg, cancellationToken);
                throw;
            }
        }

        public async Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping and removing container {ContainerId}", containerId);
            await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), cancellationToken);
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true }, cancellationToken);
            _logger.LogInformation("Container {ContainerId} stopped and removed.", containerId);
        }

        private Stream BuildContextFromDirectory(string directory)
        {
            var mem = new MemoryStream();
            var tar = TarArchive(directory);
            tar.CopyTo(mem);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        private Stream TarArchive(string sourceDir)
        {
            var mem = new MemoryStream();

            // Load ignore patterns if .dockerignore exists
            var dockerignorePath = Path.Combine(sourceDir, ".dockerignore");
            var ignorePatterns = new List<string>();

            if (File.Exists(dockerignorePath))
            {
                ignorePatterns = File.ReadAllLines(dockerignorePath)
                    .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("#"))
                    .Select(line => line.Trim())
                    .ToList();

                _logger.LogInformation("Loaded {Count} patterns from .dockerignore in {RepoPath}", ignorePatterns.Count, sourceDir);
            }

            int includedCount = 0;
            int skippedCount = 0;

            using (var tarWriter = WriterFactory.Open(mem, ArchiveType.Tar, CompressionType.None))
            {
                foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(sourceDir, file);

                    if (ShouldIgnore(relativePath, ignorePatterns))
                    {
                        _logger.LogDebug("⏭️ Skipped {File}", relativePath);
                        skippedCount++;
                        continue;
                    }

                    _logger.LogTrace("📦 Including {File}", relativePath);
                    tarWriter.Write(relativePath, file);
                    includedCount++;
                }
            }

            _logger.LogInformation("TAR build context complete: {Included} files included, {Skipped} skipped", includedCount, skippedCount);

            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        private bool ShouldIgnore(string relativePath, List<string> ignorePatterns)
        {
            foreach (var pattern in ignorePatterns)
            {
                var normalizedPattern = pattern.Replace("\\", "/").Trim();
                var normalizedPath = relativePath.Replace("\\", "/");

                if (normalizedPath.StartsWith(normalizedPattern + "/", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (normalizedPattern.Contains('*'))
                {
                    var regex = "^" + Regex.Escape(normalizedPattern).Replace("\\*", ".*") + "$";
                    if (Regex.IsMatch(normalizedPath, regex, RegexOptions.IgnoreCase))
                        return true;
                }

                if (string.Equals(normalizedPath, normalizedPattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
