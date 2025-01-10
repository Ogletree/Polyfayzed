namespace AspireApp.ApiService;

public class OrderBook
{
    public List<Order> Bids { get; set; } = [];
    public List<Order> Asks { get; set; } = [];
}