using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Interfaces
{
    public interface IDeployAppHandler
    {
        Task<AppInstance> HandleAsync(string repoUrl, int port, CancellationToken cancellationToken);
    }
}
