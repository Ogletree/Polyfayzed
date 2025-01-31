using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using AspireApp.ApiService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService.Strategy;

using System.Threading;

public class JohnyComeLately(PolyfayzedContext context, HttpClient httpClient,
    IHubContext<WebSocketHub> hubContext, JsonSerializerOptions jsonOptions, LockManager lockManager)
{
    private readonly List<GammaEvent> _events = [];
    private List<Team> _teams = [];
    private readonly List<string> _tokens = [];
    private const string LockName = "JohnyComeLatelyLock";

    public async Task ExecuteAsync(CancellationToken ctsToken)
    {
        if (!await lockManager.TryAcquireLockAsync(LockName)) return;
        try
        {
            _teams = await context.Teams.ToListAsync(ctsToken);
            while (!ctsToken.IsCancellationRequested)
            {
                await FetchEvents(ctsToken);
                await Task.Delay(10000, ctsToken);
            }
        }
        finally
        {
            lockManager.ReleaseLock(LockName);
        }
    }

    public async Task ExecuteTypeScriptFileAsync(string token)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = @"C:\Users\brian\AppData\Roaming\npm\ts-node.cmd",
            Arguments = @$"c:\repos\Polymarket\mine\createOrder.ts {token} .999 3000",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = @"c:\repos\Polymarket\mine"
        };

        using var process = new Process();
        process.StartInfo = processStartInfo;
        process.Start();
        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        Console.WriteLine(output);
        if (!string.IsNullOrEmpty(error))
        {
            Console.WriteLine($"Error: {error}");
        }
    }

    private async Task FetchEvents(CancellationToken ctsToken)
    { 
        const string url = "https://gamma-api.polymarket.com/events?&series_id=1&series_id=2&series_id=36&series_id=10002&series_id=4&limit=100&closed=false&start_time_max=2025-01-31T22:19:00.000Z";
        try
        {
            var response = await httpClient.GetStringAsync(url, ctsToken);
            var events = JsonSerializer.Deserialize<List<GammaEvent>>(response, jsonOptions);
            foreach (var gammaEvent in events)
            {
                var index = _events.FindIndex(e => e.Id == gammaEvent.Id);
                if (index != -1)
                {
                    _events[index] = gammaEvent;
                }
                else
                {
                    _events.Add(gammaEvent);
                }
            }

            var myEvents = new List<MyEvent>();
            string[] list = ["nba", "nhl"];
            foreach (var gammaEvent in _events.Where(x=> list.Contains(x.SeriesSlug)))
            {
                var myEvent = new MyEvent
                {
                    Title = gammaEvent.Title,
                    Icon = gammaEvent.Icon,
                    Elapsed = gammaEvent.Elapsed,
                    Period = gammaEvent.Period,
                    Volume = gammaEvent.Volume,
                    Liquidity = gammaEvent.Liquidity,
                    ConditionId = gammaEvent.Markets[0].ConditionId,
                    Scores = gammaEvent.Score?.Split("-"),
                    Outcomes = gammaEvent.Markets[0].Outcomes != null ? JsonSerializer.Deserialize<string[]>(gammaEvent.Markets[0].Outcomes) : null,
                    OutcomePrices = gammaEvent.Markets[0].OutcomePrices != null ? JsonSerializer.Deserialize<string[]>(gammaEvent.Markets[0].OutcomePrices) : null,
                    ClobTokenIds = gammaEvent.Markets[0].ClobTokenIds != null ? JsonSerializer.Deserialize<string[]>(gammaEvent.Markets[0].ClobTokenIds) : null,
                    GameStartTime = gammaEvent.Markets[0].GameStartTime,
                    Icons = new string[2]
                };
                // TODO. Fix this by adding league 
                myEvent.Icons[0] = _teams.FirstOrDefault(x => x.Alias == myEvent.Outcomes[0])?.Logo;
                myEvent.Icons[1] = _teams.FirstOrDefault(x => x.Alias == myEvent.Outcomes[1])?.Logo;
                myEvents.Add(myEvent);
            }

            foreach (var myEvent in myEvents)
            {
                if (myEvent.Period == "FT")
                {
                    var token = int.Parse(myEvent.Scores[0]) > int.Parse(myEvent.Scores[1]) ? myEvent.ClobTokenIds[0] : myEvent.ClobTokenIds[1];
                    if (!_tokens.Contains(token))
                    {
                        _tokens.Add(token);
                        await ExecuteTypeScriptFileAsync(token);
                    }
                }
            }

            var serialize = JsonSerializer.Serialize(myEvents);
            await hubContext.Clients.Group("JohnyComeLately").SendAsync("Events", serialize, cancellationToken: ctsToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

}

public class MyEvent
{
    public string Title { get; set; }
    public string Icon { get; set; }
    public string Elapsed { get; set; }
    public string Period { get; set; }
    public decimal Volume { get; set; }
    public decimal Liquidity { get; set; }
    public string ConditionId { get; set; }
    public string[] Scores { get; set; }
    public string[] Outcomes { get; set; }
    public string[] OutcomePrices { get; set; }
    public string[] ClobTokenIds { get; set; }
    public string[] Icons { get; set; }
    public string GameStartTime { get; set; }
}

public class GammaEvent
{
    public string Id { get; set; }
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Icon { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public bool Archived { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Volume { get; set; }
    public int EventWeek { get; set; }
    public string SeriesSlug { get; set; }
    public string Score { get; set; }
    public string Elapsed { get; set; }
    public string Period { get; set; }
    public bool Live { get; set; }
    public bool Ended { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Liquidity { get; set; }
    public List<GammaMarket> Markets { get; set; }
}
public class GammaMarket
{
    public string Id { get; set; }
    public string Question { get; set; }
    public string ConditionId { get; set; }
    public string Slug { get; set; }
    public string ResolutionSource { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Fee { get; set; }
    public string Image { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public string Outcomes { get; set; }
    public string OutcomePrices { get; set; }
    public string Volume { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public string MarketMakerAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool WideFormat { get; set; }
    public bool New { get; set; }
    public bool SentDiscord { get; set; }
    public bool Archived { get; set; }
    public string ResolvedBy { get; set; }
    public bool Restricted { get; set; }
    public string GroupItemTitle { get; set; }
    public string QuestionID { get; set; }
    public bool EnableOrderBook { get; set; }
    public decimal OrderPriceMinTickSize { get; set; }
    public int OrderMinSize { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal VolumeNum { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal LiquidityNum { get; set; }
    public string EndDateIso { get; set; }
    public string StartDateIso { get; set; }
    public bool HasReviewedDates { get; set; }
    public bool ReadyForCron { get; set; }
    public string GameStartTime { get; set; }
    public int SecondsDelay { get; set; }
    public string ClobTokenIds { get; set; }
    public bool FpmmLive { get; set; }
    public decimal VolumeClob { get; set; }
    public decimal LiquidityClob { get; set; }
    public bool AcceptingOrders { get; set; }
    public bool NegRisk { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool Ready { get; set; }
    public bool Funded { get; set; }
    public DateTime AcceptingOrdersTimestamp { get; set; }
    public bool Cyom { get; set; }
    public double Competitive { get; set; }
    public bool PagerDutyNotificationEnabled { get; set; }
    public bool Approved { get; set; }
    public int RewardsMinSize { get; set; }
    public decimal RewardsMaxSpread { get; set; }
    public decimal Spread { get; set; }
    public decimal LastTradePrice { get; set; }
    public decimal BestBid { get; set; }
    public decimal BestAsk { get; set; }
    public bool AutomaticallyActive { get; set; }
    public bool ClearBookOnStart { get; set; }
    public bool ManualActivation { get; set; }
    public bool NegRiskOther { get; set; }
}