﻿using System.Text.Json.Serialization;
using JetBrains.Annotations;
using TradeMaster.Core.Integrations.Binance.Common.Json;
using TradeMaster.Core.Integrations.Binance.Enums;

namespace TradeMaster.Core.Integrations.Binance.Responses;

public record CancelOrderResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("origClientOrderId")]
    public string OrigClientOrderId { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("orderId")]
    public long OrderId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("orderListId")]
    public long OrderListId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("clientOrderId")]
    public string ClientOrderId { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("price")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal Price { get; [UsedImplicitly]init; }

    [JsonPropertyName("origQty")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal OrigQty { get; [UsedImplicitly]init; }

    [JsonPropertyName("executedQty")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal ExecutedQty { get; [UsedImplicitly]init; }

    [JsonPropertyName("cummulativeQuoteQty")]
    [JsonConverter(typeof(StringToDecimalConverter))]
    public decimal CummulativeQuoteQty { get; [UsedImplicitly]init; }

    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; [UsedImplicitly]init; }

    [JsonPropertyName("timeInForce")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeInForce TimeInForce { get; [UsedImplicitly]init; }

    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderType Type { get; [UsedImplicitly]init; }

    [JsonPropertyName("side")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderSide Side { get; [UsedImplicitly]init; }

    [JsonPropertyName("selfTradePreventionMode")]
    public string SelfTradePreventionMode { get; [UsedImplicitly]init; } = null!;
}
