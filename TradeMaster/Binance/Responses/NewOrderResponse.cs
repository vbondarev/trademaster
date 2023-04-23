using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Responses;

public record NewOrderResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;
    
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    
    [JsonPropertyName("orderListId")]
    public int OrderListId { get; set; }
    
    [JsonPropertyName("clientOrderId")]
    public string ClientOrderId { get; set; } = null!;
    
    [JsonPropertyName("transactTime")]
    public long TransactTime { get; set; }
}
