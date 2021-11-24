using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Backend.WebApi.Model;

namespace Backend.WebApi.Data.EF;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

    public DbSet<UserInteraction> UserInteraction { get; set; }
}

