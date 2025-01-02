using AspireApp.ApiService.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace AspireApp.ApiService
{
    public class MarketUpdateService(PolyfayzedContext context, HttpClient httpClient)
    {
        public async Task IntegrateAsync()
        {
            var cursorState = await context.CursorStates.FirstOrDefaultAsync() ?? new CursorState { NextCursor = string.Empty };

            do
            {
                var response = await FetchDataFromApi(cursorState.NextCursor);
                var markets = response.data.Select(MapToMarket).ToList();

                var existingMarkets = await context.Markets
                    .Where(market => markets.Select(m => m.ConditionId).ToList().Contains(market.ConditionId))
                    .ToListAsync();
                var existingConditionIds = existingMarkets.Select(m => m.ConditionId).ToHashSet();

                var newMarkets = markets.Where(m => !existingConditionIds.Contains(m.ConditionId)).ToList();
                if (newMarkets.Any())
                {
                    context.Markets.AddRange(newMarkets);
                }
                if (cursorState.NextCursor != "LTE=")
                {
                    cursorState.NextCursor = response.next_cursor;
                    cursorState.LastUpdated = DateTime.UtcNow;
                    context.CursorStates.Update(cursorState);
                }
                await context.SaveChangesAsync();
            } while (!string.IsNullOrEmpty(cursorState.NextCursor) && cursorState.NextCursor != "LTE=");
        }

        private async Task<ApiResponse> FetchDataFromApi(string nextCursor)
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
                Tags = dto.tags?.Select(t => new Tag 
                    { Name = t }).ToList() ?? [],
                Tokens = dto.tokens?.Select(t => new Token 
                    { TokenId = t.token_id, Outcome = t.outcome, Price = t.price, Winner = t.winner }).ToList() ?? []
            };
        }
    }
}