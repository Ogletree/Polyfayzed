using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Hangfire;
using Microsoft.AspNetCore.SignalR;

namespace AspireApp.ApiService;

public class WebSocketService(IHubContext<WebSocketHub> hubContext)
{
    private const string WebSocketUrl = "wss://ws-subscriptions-clob.polymarket.com/ws/market";
    private readonly OrderBook _orderBook = new();

    [AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task StartListening(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new ClientWebSocket();
            await client.ConnectAsync(new Uri(WebSocketUrl), cancellationToken);

            const string jsonMessage = @"{""assets_ids"":[""36040360182065042048683060954028992364650049259826900000064052483130553867376""],""type"":""market""}";
            var buffer = new byte[1024 * 4];
            var messageBuffer = Encoding.UTF8.GetBytes(jsonMessage);
            var segment = new ArraySegment<byte>(messageBuffer);

            // Send the message
            await client.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
            while (client.State == WebSocketState.Open)
            {
                var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(message);
                    var serialize = JsonSerializer.Serialize(_orderBook);
                    await hubContext.Clients.All.SendAsync("ReceiveMessage", serialize, cancellationToken: cancellationToken);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private void ProcessMessage(string message)
    {
        var jsonDocument = JsonDocument.Parse(message);
        var root = jsonDocument.RootElement;

        if (root.ValueKind == JsonValueKind.Array)
        {
            foreach (var element in root.EnumerateArray())
            {
                var eventType = element.GetProperty("event_type").GetString();
                if (eventType == "book")
                {
                    UpdateOrderBook(element);
                }
                else if (eventType == "price_change")
                {
                    ApplyPriceChanges(element);
                }
            }
        }
        _orderBook.Bids = _orderBook.Bids.OrderByDescending(x => x.Price).ToList();
        _orderBook.Asks = _orderBook.Asks.OrderByDescending(x => x.Price).ToList();
    }

    private void UpdateOrderBook(JsonElement root)
    {
        _orderBook.Bids.Clear();
        _orderBook.Asks.Clear();

        foreach (var bid in root.GetProperty("bids").EnumerateArray())
        {
            _orderBook.Bids.Add(new Order
            {
                Price = decimal.Parse(bid.GetProperty("price").GetString()),
                Size = decimal.Parse(bid.GetProperty("size").GetString())
            });
        }

        foreach (var ask in root.GetProperty("asks").EnumerateArray())
        {
            _orderBook.Asks.Add(new Order
            {
                Price = decimal.Parse(ask.GetProperty("price").GetString()),
                Size = decimal.Parse(ask.GetProperty("size").GetString())
            });
        }
    }

    private void ApplyPriceChanges(JsonElement root)
    {
        foreach (var change in root.GetProperty("changes").EnumerateArray())
        {
            var price = decimal.Parse(change.GetProperty("price").GetString());
            var size = decimal.Parse(change.GetProperty("size").GetString());
            var side = change.GetProperty("side").GetString();

            if (side == "BUY")
            {
                UpdateOrderList(_orderBook.Bids, price, size);
            }
            else if (side == "SELL")
            {
                UpdateOrderList(_orderBook.Asks, price, size);
            }
        }
    }

    private void UpdateOrderList(List<Order> orders, decimal price, decimal size)
    {
        Order order = null;
        try
        {
            order = orders.FirstOrDefault(o => o.Price == price);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        if (order != null)
        {
            if (size == 0)
            {
                try
                {
                    orders.Remove(order);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            else
            {
                order.Size = size;
            }
        }
        else if (size > 0)
        {
            orders.Add(new Order { Price = price, Size = size });
        }
    }
}