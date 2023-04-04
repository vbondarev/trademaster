namespace TradeMaster;

/// <summary>
/// Функционал по рассчету риск-менеджмента
/// </summary>
internal class RiskManagementHandler
{
    public decimal CalculateStopLimitCellOrder(Trend bear, decimal orderAmount)
    {
        //Предположим, что риск-менеджмент у нас составляет 1% от суммы лимитного ордера на покупку
        var stopLimitCellPrice = orderAmount / 100;
        return stopLimitCellPrice;
    }
}
