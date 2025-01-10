using AspireApp.ApiService.Models;

namespace AspireApp.Web;

public class MarketApiClient(HttpClient httpClient)
{
    public async Task<Market[]> GetMarketsAsync(int maxItems = 100, CancellationToken cancellationToken = default)
    {
        List<Market>? markets = null;

        await foreach (var market in httpClient.GetFromJsonAsAsyncEnumerable<Market>("/markets", cancellationToken))
        {
            if (markets?.Count >= maxItems)
            {
                break;
            }

            if (market is not null)
            {
                markets ??= [];
                markets.Add(market);
            }
        }

        return markets?.ToArray() ?? [];
    }
    
    public async Task<Market> GetMarketAsync(string marketId, CancellationToken cancellationToken = default)
    {
        var market = await httpClient.GetFromJsonAsync<Market>($"/market/{marketId}", cancellationToken);
        return market ?? throw new InvalidOperationException($"Market with ID {marketId} not found.");
    }

    public async Task OpenSocketAsync(string marketId, string tokenId)
    {
        await httpClient.GetAsync($"/market/{marketId}/startSocket/{tokenId}");
    }
}