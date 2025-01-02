namespace AspireApp.ApiService;

public class WeatherForecastService
{
    public WeatherForecastService()
    {
    }

    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync()
    {
        await Task.Delay(1000);
        string[] summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    }
}