﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace AspireApp.ApiService.Models;

public partial class Market
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

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Token> Tokens { get; set; } = new List<Token>();
}