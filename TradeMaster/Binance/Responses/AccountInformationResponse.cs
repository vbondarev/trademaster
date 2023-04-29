using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace TradeMaster.Binance.Responses;

public record AccountInformationResponse
{
    [JsonPropertyName("accountType")]
    public string AccountType { get; init; } = null!;

    [JsonPropertyName("balances")]
    public IEnumerable<Balance> Balances { get; [UsedImplicitly]init; } = null!;

    public record Balance
    {
        [JsonPropertyName("asset")]
        public string Asset { get; [UsedImplicitly]init; } = null!;

        [JsonPropertyName("free")]
        public string Free { get; [UsedImplicitly]init; } = null!;

        [JsonPropertyName("locked")]
        public string Locked { get; [UsedImplicitly]init; } = null!;
    }
}
