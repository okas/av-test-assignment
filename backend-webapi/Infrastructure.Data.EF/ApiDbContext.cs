using Backend.WebApi.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Infrastructure.Data.EF;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<UserInteraction> UserInteraction { get; init; } = default!;

}
