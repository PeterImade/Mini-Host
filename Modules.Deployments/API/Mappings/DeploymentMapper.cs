using Modules.Deployments.API.DTOs;
using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.API.Mappings
{
    public class DeploymentMapper
    {
        public static DeployAppResponse ToResponseDto(AppInstance appInstance)
        {
            var deployAppResponse = new DeployAppResponse(appInstance.Id, appInstance.RepoUrl, appInstance.Status.ToString(), appInstance.ContainerId, appInstance.Domain!, appInstance.DeployedAt);

            return deployAppResponse;
        }
    }
}
