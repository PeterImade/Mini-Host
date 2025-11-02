using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Application.Interfaces
{
    public interface INginxManager
    {
        Task ConfigureReverseProxyAsync(string containerId, string domain, Port port, CancellationToken cancellationToken);
        Task RemoveReverseProxyConfigAsync(string domain, CancellationToken cancellationToken);
    }
}
