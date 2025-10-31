using Modules.Deployments.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Docker
{
    public class DockerManager: IDockerManager
    {
        public Task<string> BuildAndRunContainerAsync(string repoPath, int port, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
