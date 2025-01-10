using System.Text.Json;
using System.Text.Json.Serialization;
using AspireApp.ApiService.Models;
using Microsoft.AspNetCore.SignalR;

namespace AspireApp.ApiService.Strategy;

public class JohnyComeLately(PolyfayzedContext context, HttpClient httpClient, IHubContext<WebSocketHub> hubContext)
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
    private async Task<List<Event>> FetchEvents()
    {
        var url = @"https://gamma-api.polymarket.com/events?&series_id=1&series_id=2&series_id=36&series_id=10002&series_id=4&limit=100&closed=false&start_time_max=2025-01-11T03:40:49.714Z";
        var response = await httpClient.GetStringAsync(url);
        var events = JsonSerializer.Deserialize<List<Event>>(response);
        return events;
    }
}

public record Event
{
    [JsonPropertyName("slug")]
    public string Slug { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonPropertyName("active")]
    public bool Active { get; set; }

    [JsonPropertyName("closed")]
    public bool Closed { get; set; }

    [JsonPropertyName("archived")]
    public bool Archived { get; set; }

    [JsonPropertyName("volume")]
    public double Volume { get; set; }

    [JsonPropertyName("eventWeek")]
    public int EventWeek { get; set; }

    [JsonPropertyName("seriesSlug")]
    public string SeriesSlug { get; set; }

    [JsonPropertyName("score")]
    public string Score { get; set; }

    [JsonPropertyName("elapsed")]
    public string Elapsed { get; set; }

    [JsonPropertyName("period")]
    public string Period { get; set; }

    [JsonPropertyName("live")]
    public bool Live { get; set; }

    [JsonPropertyName("ended")]
    public bool Ended { get; set; }
}