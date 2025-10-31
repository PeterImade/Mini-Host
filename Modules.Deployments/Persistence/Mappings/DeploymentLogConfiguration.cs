using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Deployments.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modules.Deployments.Persistence.Mappings
{
    public class DeploymentLogConfiguration : IEntityTypeConfiguration<DeploymentLog>
    {
        public void Configure(EntityTypeBuilder<DeploymentLog> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(2000);
            builder.ToTable("DeploymentLogs");
        }
    }
}
