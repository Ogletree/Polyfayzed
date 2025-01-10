using AspireApp.ApiService;
using AspireApp.ApiService.Models;
using AspireApp.ApiService.Services;
using AspireApp.ApiService.Strategy;
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
builder.Services.AddScoped<MarketService>();
builder.Services.AddScoped<JohnyComeLately>();

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

using (var scope = app.Services.CreateScope())
{
    //var webSocketService = scope.ServiceProvider.GetRequiredService<WebSocketService>();
    //BackgroundJob.Enqueue(() => webSocketService.StartListening(CancellationToken.None));

    var johnyStrat = scope.ServiceProvider.GetRequiredService<JohnyComeLately>();
    BackgroundJob.Enqueue(() => johnyStrat.IntegrateAsync());

    //var marketUpdateService = scope.ServiceProvider.GetRequiredService<MarketUpdateService>();
    //RecurringJob.AddOrUpdate("MarketUpdate", () => marketUpdateService.IntegrateAsync(), Cron.Daily, new RecurringJobOptions());
}

app.Run();