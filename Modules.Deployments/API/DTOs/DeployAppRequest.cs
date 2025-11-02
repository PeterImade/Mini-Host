using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.API.DTOs
{
    public record DeployAppRequest(string RepoUrl, int Port);
}
