using System.Data.Common;
using Backend.WebApi.Infrastructure.Data.EF;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Backend.WebApi.Tests;

public class ApiLocalDbFixture : IDisposable
{
    private static readonly object _lock;
    private bool _databaseInitialized;

    public const string DefaultConnectionString = "ApiDbContext-Fixture";


    static ApiLocalDbFixture()
    {
        _lock = new();
    }

    /// <summary>
    /// Initializes <see cref="DbConnection"/> instance internaly, using <see langword="const"/> <see cref="DefaultConnectionString"/> and manages its liftime.
    /// </summary>
    public ApiLocalDbFixture() : this(DefaultConnectionString) { }

    /// <summary>
    /// Should be used for inheritance only to allow connection string injection.
    /// </summary>
    /// <remarks>
    /// When inherited, it is possible to obtain internally created and publicly exposed instance of <see cref="DbConnection"/> from <see cref="Connection"/>.
    /// </remarks>
    /// <param name="name"></param>
    protected ApiLocalDbFixture(string name)
    {
        Connection = new SqlConnection(AppSettings.Configuration.GetConnectionString(name));

        InitializeCleanDatabase();

        Connection!.Open();
    }

    /// <summary>
    /// It is kept alive throughout the liftime of the instance.
    /// </summary>
    /// <remarks>
    /// Regarding liftime, keep attention whether the instance is provided via IClassfixture<> interface or Collection("..") atribute!
    /// </remarks>
    public DbConnection? Connection { get; private set; }

    /// <summary>
    /// Create context instance with test configuration, with optional instance of <see cref="DbTransaction"/>.
    /// </summary>
    /// <param name="transaction">
    /// If omitted then instance of <see cref="DbTransaction"/> will use default transaction behavior of <see cref="DbContext"/>.
    /// </param>
    /// <returns></returns>
    public ApiDbContext CreateContext(DbTransaction? transaction = default)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApiDbContext>().UseSqlServer(Connection!);

        var context = new ApiDbContext(optionsBuilder.Options);

        if (transaction is not null)
        {
            context.Database.UseTransaction(transaction);
        }

        return context;
    }

    private void InitializeCleanDatabase()
    {
        if (_databaseInitialized)
        {
            return;
        }

        lock (_lock)
        {
            using ApiDbContext context = CreateContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.SaveChanges();
        }

        _databaseInitialized = true;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }
        if (Connection != null)
        {
            Connection.Dispose();
            Connection = null;
        }
    }
}

[CollectionDefinition("ApiLocalDbFixture")]
public class ApiLocalDbCollection : ICollectionFixture<ApiLocalDbFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
