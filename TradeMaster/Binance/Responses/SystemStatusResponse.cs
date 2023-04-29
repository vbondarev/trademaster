using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TradeMaster.Binance.Responses;

public record SystemStatusResponse
{
    [JsonPropertyName("status")]
    public int Status { get; [UsedImplicitly]init; }
    
    [JsonPropertyName("msg")]
    public string Message { get; [UsedImplicitly]init; } = null!;
}
