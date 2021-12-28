using Backend.WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args).AddServicesToContainer();

WebApplication app = builder.Build().ConfigureHttpRequestPipeline();

await app.RunAsync().ConfigureAwait(false);