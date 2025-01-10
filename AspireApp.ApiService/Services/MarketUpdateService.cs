using System.Text.Json;
using AspireApp.ApiService.Models;
using AspireApp.ApiService.Models.Dto;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace AspireApp.ApiService.Services;

public class MarketUpdateService(PolyfayzedContext context, HttpClient httpClient)
{
    private const int BatchSize = 100; // Adjust batch size as needed

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task IntegrateAsync()
    {
        var cursorState = await context.CursorStates.FirstOrDefaultAsync(x => x.Id == 1) ?? new CursorState { NextCursor = string.Empty };

        ApiResponse response;
        do
        {
            response = await FetchMarkets(cursorState.NextCursor);
            var markets = response.data.Select(MapToMarket).ToList();

            var existingMarkets = new List<Market>();
            for (int i = 0; i < markets.Count; i += BatchSize)
            {
                var batch = markets.Skip(i).Take(BatchSize).Select(m => m.ConditionId).ToList();
                var batchExistingMarkets = await context.Markets
                    .Where(market => batch.Contains(market.ConditionId))
                    .ToListAsync();
                existingMarkets.AddRange(batchExistingMarkets);
            }

            var existingConditionIds = existingMarkets.Select(m => m.ConditionId).ToHashSet();
            var newMarkets = markets.Where(m => !existingConditionIds.Contains(m.ConditionId)).ToList();
            if (newMarkets.Any())
            {
                context.Markets.AddRange(newMarkets);
            }

            foreach (var existingMarket in existingMarkets)
            {
                var updatedMarket = markets.First(m => m.ConditionId == existingMarket.ConditionId);
                existingMarket.EnableOrderBook = updatedMarket.EnableOrderBook;
                existingMarket.Active = updatedMarket.Active;
                existingMarket.Closed = updatedMarket.Closed;
                existingMarket.Archived = updatedMarket.Archived;
                existingMarket.AcceptingOrders = updatedMarket.AcceptingOrders;
                existingMarket.AcceptingOrderTimestamp = updatedMarket.AcceptingOrderTimestamp;
                existingMarket.MinimumOrderSize = updatedMarket.MinimumOrderSize;
                existingMarket.MinimumTickSize = updatedMarket.MinimumTickSize;
                existingMarket.QuestionId = updatedMarket.QuestionId;
                existingMarket.Question = updatedMarket.Question;
                existingMarket.Description = updatedMarket.Description;
                existingMarket.MarketSlug = updatedMarket.MarketSlug;
                existingMarket.EndDateIso = updatedMarket.EndDateIso;
                existingMarket.GameStartTime = updatedMarket.GameStartTime;
                existingMarket.SecondsDelay = updatedMarket.SecondsDelay;
                existingMarket.Fpmm = updatedMarket.Fpmm;
                existingMarket.MakerBaseFee = updatedMarket.MakerBaseFee;
                existingMarket.TakerBaseFee = updatedMarket.TakerBaseFee;
                existingMarket.NotificationsEnabled = updatedMarket.NotificationsEnabled;
                existingMarket.NegRisk = updatedMarket.NegRisk;
                existingMarket.NegRiskMarketId = updatedMarket.NegRiskMarketId;
                existingMarket.NegRiskRequestId = updatedMarket.NegRiskRequestId;
                existingMarket.Icon = updatedMarket.Icon;
                existingMarket.Image = updatedMarket.Image;
                existingMarket.Is5050Outcome = updatedMarket.Is5050Outcome;
                existingMarket.Tags = updatedMarket.Tags;
                existingMarket.Tokens = updatedMarket.Tokens;
            }

            if (response.next_cursor != "LTE=")
            {
                cursorState.NextCursor = response.next_cursor;
                cursorState.LastUpdated = DateTime.UtcNow;
                context.CursorStates.Update(cursorState);
                Console.WriteLine($"NextCursor: {response.next_cursor}");
            }
            await context.SaveChangesAsync();
        } while (!string.IsNullOrEmpty(cursorState.NextCursor) && response.next_cursor != "LTE=");
    }

    private async Task<ApiResponse> FetchMarkets(string nextCursor)
    {
        var url = $"https://clob.polymarket.com/markets?next_cursor={nextCursor}";
        var response = await httpClient.GetStringAsync(url);
        return JsonSerializer.Deserialize<ApiResponse>(response);
    }

    private static Market MapToMarket(MarketDto dto)
    {
        return new Market
        {
            EnableOrderBook = dto.enable_order_book,
            Active = dto.active,
            Closed = dto.closed,
            Archived = dto.archived,
            AcceptingOrders = dto.accepting_orders,
            AcceptingOrderTimestamp = dto.accepting_order_timestamp,
            MinimumOrderSize = dto.minimum_order_size,
            MinimumTickSize = dto.minimum_tick_size,
            ConditionId = dto.condition_id,
            QuestionId = dto.question_id,
            Question = dto.question,
            Description = dto.description,
            MarketSlug = dto.market_slug,
            EndDateIso = dto.end_date_iso,
            GameStartTime = dto.game_start_time,
            SecondsDelay = dto.seconds_delay,
            Fpmm = dto.fpmm,
            MakerBaseFee = dto.maker_base_fee,
            TakerBaseFee = dto.taker_base_fee,
            NotificationsEnabled = dto.notifications_enabled,
            NegRisk = dto.neg_risk,
            NegRiskMarketId = dto.neg_risk_market_id,
            NegRiskRequestId = dto.neg_risk_request_id,
            Icon = dto.icon,
            Image = dto.image,
            Is5050Outcome = dto.is_5050_outcome,
            Tags = dto.tags?.Select(t => new Tag { Name = t }).ToList() ?? [],
            Tokens = dto.tokens?.Select(t => new Token { TokenId = t.token_id, Outcome = t.outcome, Price = t.price, Winner = t.winner }).ToList() ?? []
        };
    }
}