namespace AspireApp.ApiService.Domain;

public class OrderBook
{
    public List<Order> Bids { get; set; } = [];
    public List<Order> Asks { get; set; } = [];
}