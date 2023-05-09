using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TradeMaster.Core.Integrations.Binance.Responses;

public record SymbolPriceTickerResponse
{
    [JsonPropertyName("symbol")]
    public string Symbol { get; [UsedImplicitly]init; } = null!;

    [JsonPropertyName("price")]
    public string Price { get; [UsedImplicitly]init; } = null!;
}
