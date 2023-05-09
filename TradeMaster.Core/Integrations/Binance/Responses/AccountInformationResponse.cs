using System.Text.Json.Serialization;
using JetBrains.Annotations;
using TradeMaster.Core.Binance.Common.Json;
using TradeMaster.Core.Integrations.Binance.Enums;

namespace TradeMaster.Core.Integrations.Binance.Responses;

public record AccountInformationResponse
{
    [JsonPropertyName("accountType")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AccountType AccountType { get; init; }

    [JsonPropertyName("balances")]
    public IEnumerable<Balance> Balances { get; [UsedImplicitly]init; } = null!;

    public record Balance
    {
        [JsonPropertyName("asset")]
        public string Asset { get; [UsedImplicitly]init; } = null!;

        [JsonPropertyName("free")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Free { get; [UsedImplicitly]init; }

        [JsonPropertyName("locked")]
        [JsonConverter(typeof(StringToDecimalConverter))]
        public decimal Locked { get; [UsedImplicitly]init; }
    }
}
