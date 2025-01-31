using AspireApp.ApiService;
using AspireApp.ApiService.Models;
using AspireApp.ApiService.Services;
using AspireApp.ApiService.Strategy;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    PropertyNameCaseInsensitive = true
};
builder.Services.AddSingleton(jsonOptions); 
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
builder.Services.AddScoped<TeamUpdateService>();
builder.Services.AddScoped<MarketService>();
builder.Services.AddScoped<JohnyComeLately>();
builder.Services.AddScoped<PositionUpdateService>();
builder.Services.AddSingleton<LockManager>();
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
app.MapEndpoints();
app.MapDefaultEndpoints();

var cts = new CancellationTokenSource();
app.Lifetime.ApplicationStopping.Register(() =>
{
    cts.Cancel();
});

using (var scope = app.Services.CreateScope())
{
    //var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
    //BackgroundJob.Enqueue(() => webSocketService.StartListening(CancellationToken.None));

    var johnyComeLately = scope.ServiceProvider.GetRequiredService<JohnyComeLately>();
    RecurringJob.AddOrUpdate("JohnyComeLately", () => johnyComeLately.ExecuteAsync(cts.Token), Cron.Weekly, new RecurringJobOptions());

    var positionUpdateService = scope.ServiceProvider.GetRequiredService<PositionUpdateService>();
    RecurringJob.AddOrUpdate("PositionUpdateService", () => positionUpdateService.IntegrateAsync(cts.Token), Cron.Weekly, new RecurringJobOptions());

    //var teamUpdateService = scope.ServiceProvider.GetRequiredService<TeamUpdateService>();
    //RecurringJob.AddOrUpdate("TeamUpdate", () => teamUpdateService.ExecuteAsync(), Cron.Weekly, new RecurringJobOptions());

    var marketUpdateService = scope.ServiceProvider.GetRequiredService<MarketUpdateService>();
    RecurringJob.AddOrUpdate("MarketUpdate", () => marketUpdateService.IntegrateAsync(cts.Token), Cron.Weekly, new RecurringJobOptions());
}

app.Run();