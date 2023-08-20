using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Trading.Strategies;

/// <summary>
/// Параметры для торговой стратегии
/// </summary>
public record StrategyParameter
{
    public Coin BaseCoin { get; init; }
    
    public Coin QuotedCoin { get; init; }
    
    public decimal StartAmount { get; init; }
    
    public int TotalOrderCount { get; init; }
}
