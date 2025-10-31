using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Infrastructure.Nginx
{
    public class NginxManager: INginxManager
    {
        public Task ConfigureReverseProxyAsync(string containerId, int port, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveReverseProxyConfigAsync(Port port, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
