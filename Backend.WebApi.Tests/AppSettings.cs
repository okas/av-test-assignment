using Microsoft.Extensions.Configuration;

namespace Backend.WebApi.Tests;

public static class AppSettings
{
    static AppSettings()
    {
        Configuration = new ConfigurationManager().AddJsonFile("appsettings.json").Build();
    }

    public static IConfiguration Configuration { get; }
}