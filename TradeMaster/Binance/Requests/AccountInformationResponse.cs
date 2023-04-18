using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TradeMaster.Binance.Requests;

public record AccountInformationResponse
{
    [JsonPropertyName("accountType")]
    public string AccountType { get; set; } = null!;

    [JsonPropertyName("balances")]
    public IEnumerable<Balance> Balances { get; set; } = null!;

    public record Balance
    {
        [JsonPropertyName("asset")]
        public string Asset { get; [UsedImplicitly]set; } = null!;

        [JsonPropertyName("free")]
        public string Free { get; [UsedImplicitly]set; } = null!;

        [JsonPropertyName("locked")]
        public string Locked { get; set; } = null!;
    }
}
