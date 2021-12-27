using Backend.WebApi.Model;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Data.EF;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<UserInteraction> UserInteraction { get; init; } = default!;

}
