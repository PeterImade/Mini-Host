using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Logs.Persistence.Context
{
    public class LogsDbContextFactory: IDesignTimeDbContextFactory<LogsDbContext>
    {
        public LogsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connectionString = configuration.GetConnectionString("Database");

            var optionsBuilder = new DbContextOptionsBuilder<LogsDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new LogsDbContext(optionsBuilder.Options);
        }
    }
}
