namespace AspireApp.Web;

public class MarketApiClient(HttpClient httpClient)
{
    public async Task<Market[]> GetMarketsAsync(int maxItems = 10, CancellationToken cancellationToken = default)
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
}