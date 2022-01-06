namespace Backend.WebApi.Domain.Model;

public readonly record struct WeatherForecast(
    DateTime Date,
    int TemperatureC,
    string? Summary
    )
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
