namespace AspireApp.ApiService;

public static class EndpointMappings
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/markets", async (MarketService service) =>
        {
            var markets = await service.GetMarketsAsync();
            return markets;
        }).WithName("GetMarkets");

        app.MapGet("/market/{marketId}", async (string marketId, MarketService service) =>
        {
            var market = await service.GetMarketByIdAsync(marketId);
            return market is not null ? Results.Ok(market) : Results.NotFound();
        }).WithName("GetMarketById");

        app.MapGet("/market/{marketId}/startSocket/{tokenId}", (string marketId, string tokenId, MarketService service) =>
        {
            service.OpenSocket(marketId);
        }).WithName("OpenSocket");
    }
}