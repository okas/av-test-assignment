using System.Data.Common;
using System.Reflection;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Backend.WebApi.Tests;

/// <summary>
/// Combines Depencency Injection container and Database sharing for integration testing. Uses MS.DI.
/// </summary>
public class IntegrationTestFixture : ApiLocalDbFixture, IDisposable
{
    private static readonly Assembly _apiAssembly;
    private IServiceScope? _scope;

    public new const string DefaultConnectionString = "ApiDbContext-IoC";

    static IntegrationTestFixture() => _apiAssembly = typeof(ServicesConfiguration).Assembly;

    public IntegrationTestFixture() : base(DefaultConnectionString)
    {
        ServiceCollection services = new();

        SetupLogger(services);
        SetupEntityFrameworkCore(services, Connection!);
        SetupApplication(services);
        SetupMediator(services);
        SetupAspNetControllers(services);

        RootServiceProvider = services.BuildServiceProvider(validateScopes: true);
        _scope = RootServiceProvider!.CreateScope();
        ScopedServiceProvider = _scope.ServiceProvider;
    }

    private static void SetupLogger(ServiceCollection services) => services.AddLogging();

    private static void SetupEntityFrameworkCore(ServiceCollection services, DbConnection dbConn) =>
        services.AddDbContext<ApiDbContext>(builder => { builder.UseSqlServer(dbConn); });

    private static void SetupApplication(ServiceCollection services)
    {
        services.AddTransient<
            IRequestHandler<UserInteractionGetQuery<UserInteractionDto>, (IEnumerable<UserInteractionDto>, int)>,
            UserInteractionGetQuery<UserInteractionDto>.Handler>();
    }

    private static void SetupMediator(ServiceCollection services) =>
        services.AddMediatR(_apiAssembly);

    private static void SetupAspNetControllers(ServiceCollection services) =>
        services.AddControllers()
            .AddApplicationPart(_apiAssembly)
            .AddControllersAsServices();

    public ServiceProvider? RootServiceProvider { get; private set; }


    public IServiceProvider? ScopedServiceProvider { get; private set; }

    public new void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected new virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        base.Dispose();

        if (_scope != null)
        {
            _scope.Dispose();
            _scope = null;
        }

        if (RootServiceProvider != null)
        {
            RootServiceProvider.Dispose();
            RootServiceProvider = null;
        }
    }
}

[CollectionDefinition("IntegrationTestFixture")]
public class IoCFixtureCollection : ICollectionFixture<IntegrationTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
