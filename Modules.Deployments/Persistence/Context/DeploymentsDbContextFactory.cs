using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.Configuration.Json;


namespace Modules.Deployments.Persistence.Context
{
    public class DeploymentsDbContextFactory : IDesignTimeDbContextFactory<DeploymentsDbContext>
    {
        public DeploymentsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
          .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false)
          .Build();

            var connectionString = configuration.GetConnectionString("Database");

            var optionsBuilder = new DbContextOptionsBuilder<DeploymentsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new DeploymentsDbContext(optionsBuilder.Options);
        }
    }
}
