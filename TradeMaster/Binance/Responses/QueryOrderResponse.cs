using System.Text.Json.Serialization;
using JetBrains.Annotations;
using TradeMaster.Binance.Enums;
using TradeMaster.Binance.Responses.Enums;

namespace TradeMaster.Binance.Responses;

public record QueryOrderResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("orderId")]
    public long OrderId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("orderListId")]
    public long OrderListId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("clientOrderId")]
    public string ClientOrderId { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("price")]
    public string Price { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("origQty")]
    public string OrigQty { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("executedQty")]
    public string ExecutedQty { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("cummulativeQuoteQty")]
    public string CummulativeQuoteQty { get; [UsedImplicitly]init; } = null!;

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

    [JsonPropertyName("stopPrice")]
    public string StopPrice { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("icebergQty")]
    public string IcebergQty { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("time")]
    public long Time { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("updateTime")]
    public long UpdateTime { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("isWorking")]
    public bool IsWorking { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("workingTime")]
    public long WorkingTime { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("origQuoteOrderQty")]
    public string OrigQuoteOrderQty { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("selfTradePreventionMode")]
    public string SelfTradePreventionMode { get; [UsedImplicitly]init; } = null!;
}
