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
    public class DeploymentHistoryConfiguration : IEntityTypeConfiguration<DeploymentHistory>
    {
        public void Configure(EntityTypeBuilder<DeploymentHistory> builder)
        {
            builder.HasKey(x => x.Id);
            
            builder.Property(x => x.Action).IsRequired();

            builder.Property(x => x.TimeStamp)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(x => x.PerformedBy).HasMaxLength(100);
            
            builder.HasIndex(x => x.AppInstanceId);
            
            builder.ToTable("DeploymentHistories");
        }
    }
}