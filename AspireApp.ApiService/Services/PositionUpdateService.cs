using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

namespace AspireApp.ApiService.Services;

public class PositionUpdateService(HttpClient httpClient, IHubContext<WebSocketHub> hubContext, JsonSerializerOptions jsonOptions)
{
    private static bool _shouldRun = true;

    public async Task IntegrateAsync(CancellationToken ctsToken)
    {
        _shouldRun = true;
        do
        {
            await FetchEvents(ctsToken);
            Thread.Sleep(1000 * 60);
        } while (_shouldRun);
    }

    private async Task FetchEvents(CancellationToken ctsToken)
    { 
        const string url = "https://data-api.polymarket.com/positions?user=0x974705ad5FC1A3B56f904B4881af9d7C5e8C2823";
        try
        {
            var response = await httpClient.GetStringAsync(url, ctsToken);
            var events = JsonSerializer.Deserialize<List<Position>>(response, jsonOptions);
            var serialize = JsonSerializer.Serialize(events);
            await hubContext.Clients.Group("Positions").SendAsync("Positions", serialize, cancellationToken: ctsToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    // ReSharper disable once UnusedMember.Global
    public static void StopService()
    {
        _shouldRun = false;
    }
}
public class Position
{
    public string ProxyWallet { get; set; }
    public string Asset { get; set; }
    public string ConditionId { get; set; }
    public double Size { get; set; }
    public double AvgPrice { get; set; }
    public double InitialValue { get; set; }
    public double CurrentValue { get; set; }
    public double CashPnl { get; set; }
    public double PercentPnl { get; set; }
    public double TotalBought { get; set; }
    public double RealizedPnl { get; set; }
    public double PercentRealizedPnl { get; set; }
    public double CurPrice { get; set; }
    public bool Redeemable { get; set; }
    public bool Mergeable { get; set; }
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Icon { get; set; }
    public string EventSlug { get; set; }
    public string Outcome { get; set; }
    public int OutcomeIndex { get; set; }
    public string OppositeOutcome { get; set; }
    public string OppositeAsset { get; set; }
    public string EndDate { get; set; }
    public bool NegativeRisk { get; set; }
}