namespace TradeMaster.Core.Trading.Models;

/// <summary>
/// Результат торговой операции
/// </summary>
public record TradeOperationResult
{
    /// <summary>
    /// Сумма профита
    /// </summary>
    public decimal ProfitAmount { get; init; }
    
    /// <summary>
    /// Количество базовой монеты
    /// </summary>
    public decimal BaseCoinTotal { get; init; }
    
    /// <summary>
    /// Количество котируемой монеты
    /// </summary>
    public decimal QuotedCoinTotal { get; init; }
}
