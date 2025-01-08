using AspireApp.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService;

public class MarketService(PolyfayzedContext context)
{
    public async Task<IEnumerable<Market>> GetMarketsAsync()
    {
        var markets = await context.Markets
            .Include(market => market.Tokens)
            .Include(market => market.Tags)
            .Where(market => market.Tags.Any(tag => tag.Name == "Sports"))
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
}