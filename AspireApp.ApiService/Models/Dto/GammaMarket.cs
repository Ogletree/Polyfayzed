using System.Text.Json;
using System.Text.Json.Serialization;

public class GammaMarket
{
    public string Id { get; set; }
    public string Question { get; set; }
    public string ConditionId { get; set; }
    public string Slug { get; set; }
    public string ResolutionSource { get; set; }
    public DateTime EndDate { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Liquidity { get; set; }
    public DateTime StartDate { get; set; }
    [JsonConverter(typeof(DecimalJsonConverter))]
    public decimal Fee { get; set; }
    public string Image { get; set; }
    public string Icon { get; set; }
    public string Description { get; set; }
    public string Outcomes { get; set; }
    public string OutcomePrices { get; set; }
    public string Volume { get; set; }
    public bool Active { get; set; }
    public bool Closed { get; set; }
    public string MarketMakerAddress { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool WideFormat { get; set; }
    public bool New { get; set; }
    public bool SentDiscord { get; set; }
    public bool Archived { get; set; }
    public string ResolvedBy { get; set; }
    public bool Restricted { get; set; }
    public string GroupItemTitle { get; set; }
    public string QuestionID { get; set; }
    public bool EnableOrderBook { get; set; }
    public decimal OrderPriceMinTickSize { get; set; }
    public int OrderMinSize { get; set; }
    public decimal VolumeNum { get; set; }
    public decimal LiquidityNum { get; set; }
    public string EndDateIso { get; set; }
    public string StartDateIso { get; set; }
    public bool HasReviewedDates { get; set; }
    public bool ReadyForCron { get; set; }
    public string GameStartTime { get; set; }
    public int SecondsDelay { get; set; }
    public string ClobTokenIds { get; set; }
    public bool FpmmLive { get; set; }
    public decimal VolumeClob { get; set; }
    public decimal LiquidityClob { get; set; }
    public bool AcceptingOrders { get; set; }
    public bool NegRisk { get; set; }
    public bool NotificationsEnabled { get; set; }
    public bool Ready { get; set; }
    public bool Funded { get; set; }
    public DateTime AcceptingOrdersTimestamp { get; set; }
    public bool Cyom { get; set; }
    public double Competitive { get; set; }
    public bool PagerDutyNotificationEnabled { get; set; }
    public bool Approved { get; set; }
    public int RewardsMinSize { get; set; }
    public int RewardsMaxSpread { get; set; }
    public decimal Spread { get; set; }
    public decimal LastTradePrice { get; set; }
    public decimal BestBid { get; set; }
    public decimal BestAsk { get; set; }
    public bool AutomaticallyActive { get; set; }
    public bool ClearBookOnStart { get; set; }
    public bool ManualActivation { get; set; }
    public bool NegRiskOther { get; set; }
}

public class DecimalJsonConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (decimal.TryParse(reader.GetString(), out var value))
            {
                return value;
            }
        }
        return reader.GetDecimal();
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}