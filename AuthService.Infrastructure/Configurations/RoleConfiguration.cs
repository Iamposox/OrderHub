using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Configurations;
public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();
        
        builder.Property(r => r.Rolename)
          .IsRequired();
        builder.HasIndex(r => r.Rolename)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Rolename_Unique");
    }
}
