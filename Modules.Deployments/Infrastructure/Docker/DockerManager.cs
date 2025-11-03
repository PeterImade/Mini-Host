using Docker.DotNet;
using Modules.Deployments.Application.Interfaces;
using Docker.DotNet;
using Docker.DotNet.Models;
using SharpCompress;
using SharpCompress.Archives;
using SharpCompress.Archives.Tar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpCompress.Common;
using SharpCompress.Writers;

namespace Modules.Deployments.Infrastructure.Docker
{
    public class DockerManager: IDockerManager
    {
        private readonly DockerClient _dockerClient;
        public DockerManager()
        {
            _dockerClient = new DockerClientConfiguration(new Uri("npipe://./pipe/docker_engine")).CreateClient();
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
                            Console.WriteLine(message.Stream.Trim());
                        if (!string.IsNullOrEmpty(message.ErrorMessage))
                            Console.WriteLine($"[Docker Error]: {message.ErrorMessage}");
                    });

                    await _dockerClient.Images.BuildImageFromDockerfileAsync(
                        buildParams,        // your ImageBuildParameters
                        buildContext,       // TAR stream
                        null,               // authConfigs
                        null,               // headers
                        progress,           // progress reporter
                        cancellationToken
                    );
                }

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

                return createResponse.ID;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken)
        {
            await _dockerClient.Containers.StopContainerAsync(containerId, new ContainerStopParameters(), cancellationToken);
            await _dockerClient.Containers.RemoveContainerAsync(containerId, new ContainerRemoveParameters { Force = true }, cancellationToken);
        }


        private static Stream BuildContextFromDirectory(string directory)
        {
            var mem = new MemoryStream();
            var tar = TarArchive(directory);
            tar.CopyTo(mem);
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }

        private static Stream TarArchive(string sourceDir)
        {
            var mem = new MemoryStream();

            using (var tarWriter = WriterFactory.Open(mem, ArchiveType.Tar, CompressionType.None))
            {
                foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
                {
                    var relativePath = Path.GetRelativePath(sourceDir, file);
                    tarWriter.Write(relativePath, file);
                }
            }
            mem.Seek(0, SeekOrigin.Begin);
            return mem;
        }
    }
}
