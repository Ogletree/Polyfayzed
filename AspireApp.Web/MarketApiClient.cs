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
public class Market
{
    public Guid Id { get; set; }
    public bool EnableOrderBook { get; set; }

    public bool Active { get; set; }

    public bool Closed { get; set; }

    public bool Archived { get; set; }

    public bool AcceptingOrders { get; set; }

    public DateTime? AcceptingOrderTimestamp { get; set; }

    public decimal MinimumOrderSize { get; set; }

    public decimal MinimumTickSize { get; set; }

    public string ConditionId { get; set; }

    public string QuestionId { get; set; }

    public string Question { get; set; }

    public string Description { get; set; }

    public string MarketSlug { get; set; }

    public DateTime? EndDateIso { get; set; }

    public DateTime? GameStartTime { get; set; }

    public int SecondsDelay { get; set; }

    public string Fpmm { get; set; }

    public decimal MakerBaseFee { get; set; }

    public decimal TakerBaseFee { get; set; }

    public bool NotificationsEnabled { get; set; }

    public bool NegRisk { get; set; }

    public string NegRiskMarketId { get; set; }

    public string NegRiskRequestId { get; set; }

    public string Icon { get; set; }

    public string Image { get; set; }

    public bool Is5050Outcome { get; set; }
    public List<Tag> Tags { get; set; } = new();

    public List<Token> Tokens { get; set; } = new();
}

public class Tag
{
    public string Name { get; set; }

}
public class Token
{
    public string TokenId { get; set; }

    public string Outcome { get; set; }

    public decimal Price { get; set; }

    public bool Winner { get; set; }

}