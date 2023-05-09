using JetBrains.Annotations;

namespace TradeMaster.Core.Options;

public record TradeOptions
{
    public int BuyOrderLifeMaxTimeSeconds { get; [UsedImplicitly]init; }
    
    public int SellOrderLifeMaxTimeSeconds { get; [UsedImplicitly]init; }
}
