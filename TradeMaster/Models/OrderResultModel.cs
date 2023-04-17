namespace TradeMaster.Models;

/// <summary>
/// Модель возвращаемого результата ордера
/// </summary>
public class OrderResultModel
{
    /// <summary>
    /// Сумма ордера
    /// </summary>
    public decimal CoinCount { get; set; }
    
    /// <summary>
    /// Флаг выполнения ордера
    /// </summary>
    public bool Success { get; set; }
}
