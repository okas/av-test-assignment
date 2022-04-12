using System.Reflection;
using Backend.WebApi.App.Dto;
using Backend.WebApi.App.Extensions;
using Backend.WebApi.App.Operations.UserInteractionQueries;
using Backend.WebApi.App.Swagger;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi;

public static class ServicesConfiguration
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .MemoryCacheSetup()
            .EntityFrameworkCoreSetup(builder.Configuration)
            .AspNetCoreRoutingSetup()
            .ApplicationSetup()
            .MediatRSetup()
            .ApiSetup()
            .SwaggerSetup();

        return builder;
    }

    private static IServiceCollection MemoryCacheSetup(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddSingleton(typeof(ICacheService<>), typeof(MemoryCacheService<>));

        return services;
    }

    private static IServiceCollection EntityFrameworkCoreSetup(this IServiceCollection services, ConfigurationManager config)
    {
        string connectionString = config.GetConnectionString("ApiDbContext");

        services.AddDbContext<ApiDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return services;
    }

    private static IServiceCollection AspNetCoreRoutingSetup(this IServiceCollection services)
    {
        services.AddRouting(options =>
        {
            // Client tooling feels less errorprone this way. Other than lowercase URLs are not nice.
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        return services;
    }

    private static IServiceCollection ApplicationSetup(this IServiceCollection services)
    {
        services
            .AddTransient<
                IRequestHandler<UserInteractionGetQuery<UserInteractionDto>, (IEnumerable<UserInteractionDto>, int)>,
                UserInteractionGetQuery<UserInteractionDto>.Handler>()
            .AddTransient<CUDOperationsExceptionFilter>()
            .AddTransient<OperationCancelledExceptionFilter>();

        services
            .AddScoped<IfNoneMatchFilter>()

        return services;
    }

    private static IServiceCollection MediatRSetup(this IServiceCollection services)
    {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        return services;
    }

    private static IServiceCollection ApiSetup(this IServiceCollection services)
    {
        services.AddControllers().AddOData(c =>
        {
            c.Select().Filter().OrderBy();
        });

        return services;
    }

    private static IServiceCollection SwaggerSetup(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.IncludeXmlCommentsOfCurrentProject();
            c.DocumentFilter<LowerCaseTagsDocumentFilter>();
            c.CustomOperationIdsMethodAndApiPathToSnakeCase();
        });

        return services;
    }
}
