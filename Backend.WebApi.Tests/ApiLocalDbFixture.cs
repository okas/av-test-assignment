using System;
using System.Data.Common;
using Backend.WebApi.Infrastructure.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Backend.WebApi.Tests;

public sealed class ApiLocalDbFixture : IDisposable
{
    private static readonly string _localDbConnectionString;
    private static readonly object _lock;
    private bool _databaseInitialized;

    static ApiLocalDbFixture()
    {
        _localDbConnectionString = @"Server=(localdb)\mssqllocaldb;Database=av-test-assignment_webapi-tests;Trusted_Connection=True";
        _lock = new();
    }

    public ApiLocalDbFixture()
    {
        Connection = new SqlConnection(_localDbConnectionString);
        InitializeCleanDatabase();
        Connection.Open();
    }

    /// <summary>
    /// It is kept alive throughout the liftime of the instance.
    /// </summary>
    /// <remarks>
    /// Regarding liftime, keep attention whether the instance is provided via IClassfixture<> interface or Collection("..") atribute!
    /// </remarks>
    public DbConnection Connection { get; }

    /// <summary>
    /// Create context instance with test configuration, with optional instance of <see cref="DbTransaction"/>.
    /// </summary>
    /// <param name="transaction">
    /// If omitted then instance of <see cref="ApiDbContext"/> will use default transaction behavior of <see cref="DbContext"/>.
    /// </param>
    /// <returns></returns>
    public ApiDbContext CreateContext(DbTransaction? transaction = default)
    {
        var modelBuilder = new DbContextOptionsBuilder<ApiDbContext>().UseSqlServer(Connection);

        var context = new ApiDbContext(modelBuilder.Options);

        if (transaction is not null)
        {
            context.Database.UseTransaction(transaction);
        }

        return context;
    }

    private void InitializeCleanDatabase()
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

[CollectionDefinition("ApiLocalDbFixture")]
public class ApiLocalDbCollection : ICollectionFixture<ApiLocalDbFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}