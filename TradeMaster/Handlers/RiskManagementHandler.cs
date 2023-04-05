namespace TradeMaster;

/// <summary>
/// Функционал по рассчету риск-менеджмента
/// </summary>
internal class RiskManagementHandler
{
    /// <summary>
    /// Рассчет стоимости стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="bear"></param>
    /// <param name="orderAmount"></param>
    /// <returns></returns>
    public decimal CalculateStopLimitCellOrder(Trend bear, decimal orderAmount, decimal coinCount)
    {
        //Предположим, что риск-менеджмент у нас составляет 1% от суммы лимитного ордера на покупку
        
        //Сумма, которая должна сохраниться на кошельке с учетом потери 1%
        var stopLimitAmount = orderAmount - (orderAmount / 100);
        
        //Получаем цену стоп-лимитного ордера
        var stopLimitCellPrice = stopLimitAmount / coinCount;
        
        
        
        //Перерасчет делается после выполнения лимитного ордера на продажу
        //Необходимо сделать перерасчет 
        
        
        return stopLimitCellPrice;
    }
}
