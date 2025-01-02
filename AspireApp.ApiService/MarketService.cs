using AspireApp.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService;

public class MarketService(PolyfayzedContext context)
{
    public async Task<IEnumerable<Market>> GetMarketsAsync()
    {
        var marketId = Guid.Parse("f62ddfc4-75b2-4d2c-85f5-00fb1f0cd208");
        var markets = await context.Markets
            .Include(market => market.Tokens)
            //.Include(market => market.Tags)
            .Where(x => x.Id == marketId)
            .ToListAsync();
        return markets;
    }
}