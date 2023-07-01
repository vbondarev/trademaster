using System.Text.Json.Serialization;
using JetBrains.Annotations;
using TradeMaster.Core.Integrations.Binance.Enums;

namespace TradeMaster.Core.Integrations.Binance.Responses;

public record BuyOrderResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("orderId")]
    public int OrderId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("orderListId")]
    public int OrderListId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("clientOrderId")]
    public string ClientOrderId { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("status")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderStatus Status { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("type")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderType Type { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("side")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public OrderSide Side { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("transactTime")]
    public long TransactTime { get; [UsedImplicitly]init; }
}
