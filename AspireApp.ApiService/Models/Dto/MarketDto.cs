// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
namespace AspireApp.ApiService.Models.Dto;

public class MarketDto
{
    public bool enable_order_book { get; set; }
    public bool active { get; set; }
    public bool closed { get; set; }
    public bool archived { get; set; }
    public bool accepting_orders { get; set; }
    public DateTime? accepting_order_timestamp { get; set; }
    public decimal minimum_order_size { get; set; }
    public decimal minimum_tick_size { get; set; }
    public string condition_id { get; set; }
    public string question_id { get; set; }
    public string question { get; set; }
    public string description { get; set; }
    public string market_slug { get; set; }
    public DateTime? end_date_iso { get; set; }
    public DateTime? game_start_time { get; set; }
    public int seconds_delay { get; set; }
    public string fpmm { get; set; }
    public decimal maker_base_fee { get; set; }
    public decimal taker_base_fee { get; set; }
    public bool notifications_enabled { get; set; }
    public bool neg_risk { get; set; }
    public string neg_risk_market_id { get; set; }
    public string neg_risk_request_id { get; set; }
    public string icon { get; set; }
    public string image { get; set; }
    public bool is_5050_outcome { get; set; }
    public List<string> tags { get; set; }
    public List<TokenDto> tokens { get; set; }
}