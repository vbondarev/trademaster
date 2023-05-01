using JetBrains.Annotations;

namespace TradeMaster.Options;

public record TradeOptions
{
    public int BuyOrderLifeMaxTimeSeconds { get; [UsedImplicitly]init; }
}
