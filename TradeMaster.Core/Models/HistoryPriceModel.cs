namespace TradeMaster.Core.Models;

/// <summary>
/// Модель для истории изменений цены в определенном интервале
/// </summary>
public class HistoryPriceModel
{
    /// <summary>
    /// Наименование временного интервала
    /// </summary>
    public Interval Interval { get; set; }
    
    /// <summary>
    /// Количество интервалов
    /// </summary>
    public int IntervalCount { get; set; }
    
    /// <summary>
    /// Список верхних и нижних границ стоимости монеты в заданом интервале
    /// </summary>
    public List<CostLimits> CostLimits { get; set; }
}
