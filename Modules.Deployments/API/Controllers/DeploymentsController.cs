using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modules.Deployments.API.DTOs;
using Modules.Deployments.API.Mappings;
using Modules.Deployments.Application.Commands.DeployApp;
using Modules.Deployments.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.API.Controllers
{

    [ApiController]
    [Route("api/deployments")]
    public class DeploymentsController: ControllerBase
    {
        private readonly IDeployAppHandler _deployAppHandler;

        public DeploymentsController(IDeployAppHandler deployAppHandler)
        {
            _deployAppHandler = deployAppHandler;
        }

        [HttpPost("deploy")]
        [ProducesResponseType(typeof(DeployAppResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> DeployApp([FromBody] DeployAppRequest request, CancellationToken cancellationToken)
        {
            var appInstance = await _deployAppHandler.HandleAsync(request.RepoUrl, request.Port, cancellationToken);

            var response = DeploymentMapper.ToResponseDto(appInstance);

            return CreatedAtAction(nameof(DeployApp), new { id = response.Id }, response);
        }
    }
}
