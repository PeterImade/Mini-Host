using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Domain.Enums
{
    public enum DeploymentStatus
    {
        Pending,
        Running,
        Completed,
        Failed,
        Stopped
    }
}
