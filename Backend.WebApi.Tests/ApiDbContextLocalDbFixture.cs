using System;
using System.Data.Common;
using Backend.WebApi.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi.Tests;

public class ApiDbContextLocalDbFixture : IDisposable
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public ApiDbContextLocalDbFixture()
    {
        Connection = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=EFTestSample;Trusted_Connection=True");
        Seed();
        Connection.Open();
    }
    public DbConnection Connection { get; }

    public ApiDbContext CreateContext(DbTransaction transaction = null)
    {
        var modelBuilder = new DbContextOptionsBuilder<ApiDbContext>().UseSqlServer(Connection);

        var context = new ApiDbContext(modelBuilder.Options);

        if (transaction != null)
        {
            context.Database.UseTransaction(transaction);
        }

        return context;
    }

    private void Seed()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    context.SaveChanges();
                }
                _databaseInitialized = true;
            }
        }
    }
    public void Dispose() => Connection.Dispose();
}