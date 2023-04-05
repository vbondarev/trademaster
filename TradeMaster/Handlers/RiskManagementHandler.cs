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
    /// <param name="coinCount"></param>
    /// <returns></returns>
    public decimal CalculateStopLimitCellOrder(Trend bear, decimal startAmount, decimal coinCount, decimal profitAmount, decimal buyPrice)
    {
        //Включаем в риск-менеджмент сумму профита
        var profitCount = profitAmount / buyPrice;
            
        //Предположим, что риск-менеджмент у нас составляет 1% от суммы лимитного ордера на покупку
        //Сумма, которая должна сохраниться на кошельке с учетом потери 1%
        var stopLimitAmount = startAmount - (startAmount / 100);
        
        //Получаем цену стоп-лимитного ордера
        
        //!!!Важно проверить
        var stopLimitCellPrice = stopLimitAmount / (coinCount + profitCount);
        
        
        
        //Перерасчет делается после выполнения лимитного ордера на продажу
        //Необходимо сделать перерасчет 
        
        
        return stopLimitCellPrice;
    }
}
