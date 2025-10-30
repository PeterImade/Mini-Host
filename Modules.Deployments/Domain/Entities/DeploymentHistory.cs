using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Entities
{
    public class DeploymentHistory
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AppInstanceId { get; private set; }
        public string Action { get; private set; }  // e.g. "Deployed", "Restarted"
        public DateTime TimeStamp { get; private set; }
        public string PerformedBy { get; set; }

        public DeploymentHistory(Guid appInstanceId, string action, string performedBy)
        {
            AppInstanceId = appInstanceId;
            Action = action;
            PerformedBy = performedBy;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
