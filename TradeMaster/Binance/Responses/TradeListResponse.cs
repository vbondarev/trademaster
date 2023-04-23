using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Responses;

public record TradeListResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;
    
    [JsonPropertyName("id")]
    public string Id { get; set; } = null!;
    
    [JsonPropertyName("orderId")]
    public int OrderId { get; set; }
    
    [JsonPropertyName("orderListId")]
    public int OrderListId { get; set; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("qty")]
    public decimal Quantity { get; set; }
    
    [JsonPropertyName("quoteQty")]
    public decimal QuoteQuantity { get; set; }
    
    [JsonPropertyName("commission")]
    public string Commission { get; set; } = null!;
    
    [JsonPropertyName("commissionAsset")]
    public string CommissionAsset { get; set; } = null!;

    [JsonPropertyName("time")]
    public long TransactTime { get; set; }
    
    [JsonPropertyName("isBuyer")]
    public bool IsBuyer { get; set; }
    
    [JsonPropertyName("isMaker")]
    public bool IsMaker { get; set; }
    
    [JsonPropertyName("isBestMatch")]
    public bool IsBestMatch { get; set; }
}
