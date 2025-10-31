using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Interfaces
{
    public interface IDockerManager
    {
        Task<string> BuildAndRunContainerAsync(string repoPath, int port, CancellationToken cancellationToken);
        Task StopAndRemoveContainerAsync(string containerId, CancellationToken cancellationToken);
    }
}
