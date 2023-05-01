using JetBrains.Annotations;

namespace TradeMaster.Options;

public record TradeOptions
{
    public int BuyOrderLifeMaxTimeSeconds { get; [UsedImplicitly]init; }
    
    public int SellOrderLifeMaxTimeSeconds { get; [UsedImplicitly]init; }
}
