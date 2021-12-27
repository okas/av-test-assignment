using System.Reflection;
using Backend.WebApi.App.Services;
using Backend.WebApi.App.Swagger;
using Backend.WebApi.Infrastructure.Data.EF;
using MediatR;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder? builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApiDbContext"));
});

// Add services to the container.
builder.Services.Configure<RouteOptions>(options =>
{
    // Client tooling feels less errorprone this way. Other than lowercase URLs are not nice.
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;
});

builder.Services.AddScoped<UserInteractionService>();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

builder.Services.AddControllers().AddOData(c =>
{
    c.Select().Filter().OrderBy();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.IncludeXmlCommentsOfCurrentProject();
    c.DocumentFilter<LowerCaseTagsDocumentFilter>();
    c.CustomOperationIdsMethodAndApiPathToSnakeCase();
});

WebApplication? app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.DisplayOperationId();
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
