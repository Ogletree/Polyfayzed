using AspireApp.ApiService.Models;
using AspireApp.ApiService.Services;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService;

public class MarketService(PolyfayzedContext context, WebSocketService webSocketService, IBackgroundJobClient hangfireClient)
{
    public async Task<IEnumerable<Market>> GetMarketsAsync()
    {
        var markets = await context.Markets
            .Include(market => market.Tokens)
            .Include(market => market.Tags)
            .Where(market => market.Question.Contains("vs.") && market.Closed == false && market.Tags.Any(tag => tag.Name == "Sports") && market.Tags.Any(tag => tag.Name == "NBA")).Take(50)
            .ToListAsync();
        return markets;
    }

    public async Task<Market> GetMarketByIdAsync(string marketId)
    {
        var markets = await context.Markets
            .Include(market => market.Tokens)
            .Include(market => market.Tags)
            .Where(market => market.ConditionId == marketId)
            .FirstOrDefaultAsync();
        return markets;
    }

    public bool OpenSocket(string tokenId)
    {
        hangfireClient.Enqueue(() => webSocketService.StartListening(CancellationToken.None));
        return true;
    }
}