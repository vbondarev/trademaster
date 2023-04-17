namespace TradeMaster.Models;

/// <summary>
/// Предварительная модель результатов работы приложения
/// </summary>
public class ResultStatisticModel
{
    /// <summary>
    /// Сумма профита
    /// </summary>
    public decimal ProfitAmount { get; set; }
    
    /// <summary>
    /// Количество базовой монеты
    /// </summary>
    public decimal BaseCoinTotal { get; set; }
    
    /// <summary>
    /// Количество котируемой монеты
    /// </summary>
    public decimal QuotedCoinTotal { get; set; }
}
