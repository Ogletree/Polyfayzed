using AspireApp.ApiService;
using AspireApp.ApiService.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add HangFire services
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddHangfireServer();

// Add EntityFramework services
builder.Services.AddDbContext<PolyfayzedContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register the DataFetchService
builder.Services.AddTransient<DataFetchService>();
builder.Services.AddScoped<WeatherForecastService>();

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

app.MapDefaultEndpoints();

using (var scope = app.Services.CreateScope())
{
    var dataFetchService = scope.ServiceProvider.GetRequiredService<DataFetchService>();
    RecurringJob.AddOrUpdate("DataFetchJob", () => dataFetchService.FetchAndStoreDataAsync(), Cron.Daily, new RecurringJobOptions());
}

app.Run();