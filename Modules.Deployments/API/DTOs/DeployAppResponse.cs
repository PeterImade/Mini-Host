using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.API.DTOs
{
    public record DeployAppResponse(Guid Id, string repoUrl, string status, string containerId, DateTime DeployedAt);  
}
