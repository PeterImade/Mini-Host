using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Entities
{
    public class DeploymentLog
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public Guid AppInstanceId { get; private set; }
        public string Message { get; private set; }
        public DateTime TimeStamp { get; private set; }

        public DeploymentLog(Guid appInstanceId, string message)
        {
            AppInstanceId = appInstanceId;
            Message = message;
            TimeStamp = DateTime.UtcNow;
        }
    }
}
