using Backend.WebApi;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddServices();

WebApplication app = builder.Build();
app.SetupRequestPipeline();

await app.RunAsync().ConfigureAwait(false);