using System.Text.Json.Serialization;
using JetBrains.Annotations;

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
    
    [JsonPropertyName("transactTime")]
    public long TransactTime { get; [UsedImplicitly]init; }
}
