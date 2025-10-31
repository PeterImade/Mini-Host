using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Deployments.Domain.Entities;
using Modules.Deployments.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Modules.Deployments.Domain.ValueObjects;

namespace Modules.Deployments.Persistence.Mappings
{
    public class AppInstanceConfiguration : IEntityTypeConfiguration<AppInstance>
    {
        public void Configure(EntityTypeBuilder<AppInstance> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RepoUrl)
                .HasConversion(url => url.Value, value => new RepoUrl(value))
                .HasMaxLength(500);
            builder.Property(x => x.Port)
                .HasConversion(port => port.Value, value => new Port(value));
            builder.Property(x => x.Status)
                .HasConversion<string>();
            builder.Property(x => x.DeployedAt).IsRequired();
            builder.ToTable("AppInstances");
        }
    }
}
