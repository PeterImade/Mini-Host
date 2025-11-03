using Microsoft.EntityFrameworkCore;
using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Logs.Persistence.Context
{
    public class LogsDbContext: DbContext
    {
        public LogsDbContext(DbContextOptions<LogsDbContext> options) : base(options) { }

        public DbSet<DeploymentLog> DeploymentLogs => Set<DeploymentLog>();
        public DbSet<DeploymentHistory> DeploymentHistories => Set<DeploymentHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LogsDbContext).Assembly);
        }
    }
}
