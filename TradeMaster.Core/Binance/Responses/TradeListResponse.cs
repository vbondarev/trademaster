using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TradeMaster.Core.Binance.Responses;

public record TradeListResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("id")]
    public string Id { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("orderId")]
    public int OrderId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("orderListId")]
    public int OrderListId { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("price")]
    public decimal Price { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("qty")]
    public decimal Quantity { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("quoteQty")]
    public decimal QuoteQuantity { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("commission")]
    public string Commission { get; [UsedImplicitly]init; } = null!;
    
    [JsonPropertyName("commissionAsset")]
    public string CommissionAsset { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("time")]
    public long TransactTime { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("isBuyer")]
    public bool IsBuyer { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("isMaker")]
    public bool IsMaker { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("isBestMatch")]
    public bool IsBestMatch { get; [UsedImplicitly]init; }
}
