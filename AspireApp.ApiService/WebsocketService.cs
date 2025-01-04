﻿using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.SignalR;

public class WebSocketService
{
    private readonly IHubContext<WebSocketHub> _hubContext;
    private readonly string _webSocketUrl = "wss://ws-subscriptions-clob.polymarket.com/ws/market";

    public WebSocketService(IHubContext<WebSocketHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task StartListening(CancellationToken cancellationToken)
    {
        using var client = new ClientWebSocket();
        await client.ConnectAsync(new Uri(_webSocketUrl), cancellationToken);

        var jsonMessage =
            @"{""assets_ids"":[""106298861292246161053429398983207070416337091865277428512610132653037294562229""],""type"":""market""}";
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
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            }
        }
    }
}