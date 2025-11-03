using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Persistence.Context
{
    public class DeploymentsDbContext: DbContext
    {
        public DeploymentsDbContext(DbContextOptions<DeploymentsDbContext> options): base(options)
        {
            
        }

        public DbSet<AppInstance> AppInstances { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DeploymentsDbContext).Assembly);
        }
    }
}
