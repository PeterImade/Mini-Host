using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Events
{
    public class AppDeployedEvent
    {
        public Guid AppId { get; }
        public DateTime DeployedAt { get; }
        public AppDeployedEvent(Guid appId, DateTime deployedAt)
        {
            AppId = appId;
            DeployedAt = deployedAt;
        }
    }
}
