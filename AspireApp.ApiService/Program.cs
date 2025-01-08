using AspireApp.ApiService;
using AspireApp.ApiService.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}); 
builder.Services.AddRazorComponents();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WebSocketService>();


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
app.UseCors();
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

// Map SignalR hub
app.MapHub<WebSocketHub>("/websockethub");

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
    var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
    BackgroundJob.Enqueue(() => webSocketService.StartListening(CancellationToken.None));

    var marketUpdateService = scope.ServiceProvider.GetRequiredService<MarketUpdateService>();
    RecurringJob.AddOrUpdate("MarketUpdate", () => marketUpdateService.IntegrateAsync(), Cron.Daily, new RecurringJobOptions());
}

app.Run();