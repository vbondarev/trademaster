using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Responses;

public record SymbolPriceTickerResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; set; } = null!;

    [JsonPropertyName("price")]
    public string Price { get; set; } = null!;
}
