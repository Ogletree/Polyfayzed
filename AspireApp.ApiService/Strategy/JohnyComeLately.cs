using System.Text.Json;
using System.Text.Json.Serialization;
using AspireApp.ApiService.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspireApp.ApiService.Strategy;

public class JohnyComeLately(PolyfayzedContext context, HttpClient httpClient, IHubContext<WebSocketHub> hubContext, JsonSerializerOptions jsonOptions)
{
    public async Task IntegrateAsync()
    {
        do
        {
            var events = FetchEvents();
            var serialize = JsonSerializer.Serialize(events);
            await hubContext.Clients.Group("JohnyComeLately").SendAsync("ReceiveMessage", serialize);
            Thread.Sleep(1000 * 10); // 10 seconds
        } while (true);
    }
    private async Task FetchEvents()
    { //"https://gamma-api.polymarket.com/events?&series_id=1&series_id=2&series_id=36&series_id=10002&series_id=4&limit=100&closed=false&start_time_max=2025-01-17T21:04:00.000Z";
        const string url = "https://gamma-api.polymarket.com/events?&series_id=2&limit=100&closed=false&start_time_max=2025-01-17T21:04:00.000Z";
        try
        {
            return;
            var response = await httpClient.GetStringAsync(url);
            var events = JsonSerializer.Deserialize<List<GammaEvent>>(response, jsonOptions);
            Console.WriteLine("Event count: " + events.Count);
            var serialize = JsonSerializer.Serialize(events);
            await hubContext.Clients.Group("JohnyComeLately").SendAsync("Events", serialize);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}

public class GammaEvent
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Icon { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public bool Archived { get; set; }
    public double Volume { get; set; }
    public int EventWeek { get; set; }
    public string SeriesSlug { get; set; }
    public string Score { get; set; }
    public string Elapsed { get; set; }
    public string Period { get; set; }
    public bool Live { get; set; }
    public bool Ended { get; set; }
    public List<GammaMarket> Markets { get; set; }
}