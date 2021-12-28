using System.Reflection;
using Backend.WebApi.App.Services;
using Backend.WebApi.App.Swagger;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebApi;

public static class ProgramDependencyInjection
{
    public static WebApplicationBuilder AddServicesToContainer(this WebApplicationBuilder builder)
    {
        return builder.EntityFrameworkCoreSetup()
            .AspNetCoreRoutingSetup()
            .ApplicationSetup()
            .MediatRSetup()
            .ODataSetup()
            .SwaggerSetup();
    }

    private static WebApplicationBuilder EntityFrameworkCoreSetup(this WebApplicationBuilder builder)
    {
        string connectionString = builder.Configuration.GetConnectionString("ApiDbContext");

        builder.Services.AddDbContext<ApiDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        return builder;
    }

    private static WebApplicationBuilder AspNetCoreRoutingSetup(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RouteOptions>(options =>
        {
            // Client tooling feels less errorprone this way. Other than lowercase URLs are not nice.
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });

        return builder;
    }

    private static WebApplicationBuilder ApplicationSetup(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<UserInteractionService>();

        return builder;
    }

    private static WebApplicationBuilder MediatRSetup(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        return builder;
    }

    private static WebApplicationBuilder ODataSetup(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddOData(c =>
        {
            c.Select().Filter().OrderBy();
        });

        return builder;
    }

    private static WebApplicationBuilder SwaggerSetup(this WebApplicationBuilder builder)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            c.IncludeXmlCommentsOfCurrentProject();
            c.DocumentFilter<LowerCaseTagsDocumentFilter>();
            c.CustomOperationIdsMethodAndApiPathToSnakeCase();
        });

        return builder;
    }
}
