using System;
using System.Data.Common;
using Backend.WebApi.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.WebApi.Tests;

public class ApiDbContextLocalDbFixture : IDisposable
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public ApiDbContextLocalDbFixture()
    {
        Connection = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=av-test-assignment_webapi-tests;Trusted_Connection=True");
        Seed();
        Connection.Open();
    }

    public DbConnection Connection { get; }

    /// <summary>
    /// Optional. Use transactions in Facts/Theories to roll back data in test if it is necessary!
    /// </summary>
    /// <param name="transaction">Transaction instance, that is given to EF.</param>
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

[CollectionDefinition("ApiDbContext")]
public class ApiDbContextCollection : ICollectionFixture<ApiDbContextLocalDbFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}