using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Modules.Deployments.Infrastructure.Services;
using Modules.Logs.Application.Interfaces; 
using Modules.Logs.Persistence.Context;
using Shared.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Logs
{
    public class LogsModule: IModule
    {
        public void RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDeploymentLogService, DeploymentLogService>();

            services.AddDbContext<LogsDbContext>(options =>
               options.UseSqlServer(configuration.GetConnectionString("PaasDatabase"),
                   sql => sql.MigrationsAssembly(typeof(LogsDbContext).Assembly.FullName)));
        }
    }
}
