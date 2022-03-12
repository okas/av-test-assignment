using Backend.WebApi.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Backend.WebApi.Infrastructure.Data.EF.Schema;

namespace Backend.WebApi.Infrastructure.Data.EF;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    { }

    public DbSet<UserInteraction> UserInteraction { get; init; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfiguration(new EntityTypeConfigurationUserInteraction());
}
