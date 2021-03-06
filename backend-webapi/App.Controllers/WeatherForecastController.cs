using Backend.WebApi.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Backend.WebApi.App.Controllers
{
    /// <summary>
    /// Endpoint for autogenerated testdata.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] _summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild",
            "Warm", "Balmy", "Hot", "Sweltering", "Scorching",
        };

        /// <summary>
        /// Get all weatherforecasts.
        /// </summary>
        /// <returns></returns>
        [HttpGet(Name = "getWeatherForecast")]
        [EnableQuery]
#pragma warning disable MA0038 // Make method static
        public IEnumerable<WeatherForecast> Get()
#pragma warning restore MA0038 // Make method static
        {
            return Enumerable.Range(1, 50).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = _summaries[Random.Shared.Next(_summaries.Length)],
            })
            .ToArray();
        }
    }
}
