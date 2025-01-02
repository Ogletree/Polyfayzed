public class WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public DateOnly Date { get; init; } = Date;
    public int TemperatureC { get; init; } = TemperatureC;
    public string? Summary { get; init; } = Summary;

    public void Deconstruct(out DateOnly Date, out int TemperatureC, out string? Summary)
    {
        Date = this.Date;
        TemperatureC = this.TemperatureC;
        Summary = this.Summary;
    }
}