using AspireApp.ApiService;
using AspireApp.ApiService.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

builder.Services.AddDbContext<PolyfayzedContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<MarketUpdateService>();
builder.Services.AddScoped<WeatherForecastService>();
builder.Services.AddScoped<MarketService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PolyfayzedContext>();
    dbContext.Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHangfireDashboard();

app.MapGet("/weatherforecast", async (WeatherForecastService service) =>
    {
        var forecast = await service.GetWeatherForecastsAsync();
        return forecast;
    })
    .WithName("GetWeatherForecast");
app.MapGet("/markets", async (MarketService service) =>
    {
        var markets = await service.GetMarketsAsync();
        return markets;
    })
    .WithName("GetMarkets");

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dataFetchService = scope.ServiceProvider.GetRequiredService<MarketUpdateService>();
    RecurringJob.AddOrUpdate("MarketUpdate", () => dataFetchService.IntegrateAsync(), Cron.Daily, new RecurringJobOptions());
}

app.Run();