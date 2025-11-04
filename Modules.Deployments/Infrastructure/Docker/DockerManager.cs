using Docker.DotNet;
using Modules.Deployments.Application.Interfaces;
using Docker.DotNet.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Writers;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Modules.Deployments.Infrastructure.Docker
{
    public class DockerManager : IDockerManager
    {
        private readonly DockerClient _dockerClient;
        private readonly ILogger<DockerManager> _logger;

        public DockerManager(ILogger<DockerManager> logger)
        {
            _logger = logger;
            _dockerClient = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine")
            ).CreateClient();
        }

        public async Task<string> BuildAndRunContainerAsync(string repoPath, int port, CancellationToken cancellationToken)
        {
            try
            {
                string imageTag = $"app_{Guid.NewGuid()}";

                using (var buildContext = BuildContextFromDirectory(repoPath))
                {
                    var buildParams = new ImageBuildParameters
                    {
                        Tags = new[] { imageTag },
                    };

                    var progress = new Progress<JSONMessage>(message =>
                    {
                        if (!string.IsNullOrEmpty(message.Stream))
                            _logger.LogInformation("[Docker] {Message}", message.Stream.Trim());

                        if (!string.IsNullOrEmpty(message.ErrorMessage))
                            _logger.LogError("[Docker Error] {Error}", message.ErrorMessage);
                    });

                    _logger.LogInformation("Starting Docker image build for {RepoPath}", repoPath);

                    await _dockerClient.Images.BuildImageFromDockerfileAsync(
                        buildParams,
                        buildContext,
                        null,
                        null,
                        progress,
                        cancellationToken
                    );
                }

                _logger.LogInformation("Image built successfully. Creating container on port {Port}...", port);

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

                _logger.LogInformation("Container started successfully. ID: {ContainerId}", createResponse.ID);

                return createResponse.ID;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during Docker build or container start for repo: {RepoPath}", repoPath);
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
