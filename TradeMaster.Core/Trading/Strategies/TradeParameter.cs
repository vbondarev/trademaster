using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Trading.Strategies;

/// <summary>
/// Параметры для торгового тренда
/// </summary>
public record TradeParameter
{
    public Coin BaseCoin { get; init; }
    
    public Coin QuotedCoin { get; init; }
    
    public decimal StartAmount { get; init; }
    
    public int TotalOrderCount { get; init; }
}
