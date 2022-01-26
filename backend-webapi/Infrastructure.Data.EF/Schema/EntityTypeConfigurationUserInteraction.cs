using Backend.WebApi.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Backend.WebApi.Infrastructure.Data.EF.Schema;

public class EntityTypeConfigurationUserInteraction : IEntityTypeConfiguration<UserInteraction>
{
    public void Configure(EntityTypeBuilder<UserInteraction> builder)
    {
        builder.Property(e => e.Description).IsRequired().HasMaxLength(300);

        builder.Property(e => e.RowVer).IsRowVersion();
    }
}
