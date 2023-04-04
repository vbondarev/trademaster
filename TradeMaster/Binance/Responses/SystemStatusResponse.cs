using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Responses;

public record SystemStatusResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }
    
    [JsonPropertyName("msg")]
    public string Message { get; set; } = null!;
}
