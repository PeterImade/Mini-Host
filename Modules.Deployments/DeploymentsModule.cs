using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Deployments.Application.Interfaces;
using Modules.Deployments.Persistence.Repositories;
using Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments
{
    public class DeploymentsModule: IModule
    {
        public void RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDeploymentRepository, DeploymentRepository>();
        }
    }
}
