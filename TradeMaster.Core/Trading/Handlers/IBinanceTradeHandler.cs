using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Trading.Handlers;

internal interface IBinanceTradeHandler
{
    /// <summary>
    /// Расчет суммы ордера
    /// </summary>
    Task<decimal> CalculateOrderAmount(Coin quotedCoin);
    
    /// <summary>
    /// Формирование истории изменений цены в заданном диапазоне
    /// </summary>
    Task<HistoryPriceModel> GeneratePriceHistory(Coin baseCoin, Coin quotedCoin, Interval interval, int intervalCount);

    /// <summary>
    /// Расчет стоимости ордера на покупку
    /// </summary>
    decimal CalculateBuyOrderPrice(Trend trend, HistoryPriceModel historyPrice);
    
    /// <summary>
    /// Расчет стоимости ордера на продажу
    /// </summary>
    decimal CalculateSellOrderPrice(Coin baseCoin, Coin quotedCoin, Trend trend, HistoryPriceModel priceHistory, decimal price);

    Task<OrderOperation> CreateBuyOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity);
    
    /// <summary>
    /// Отмена ордера
    /// </summary>
    Task<OrderOperation> CancelBuyOrder(Coin baseCoin, Coin quotedCoin);

    /// <summary>
    /// Получение стоп ордера на продажу
    /// </summary>
    Task<OrderOperation> GetSellStopOrder(Coin baseCoin, Coin quotedCoin);

    /// <summary>
    /// Отмена стоп ордера на продажу
    /// </summary>
    Task<OrderOperation> CancelSellStopOrder(Coin baseCoin, Coin quotedCoin);

    /// <summary>
    /// Создание ордера на продажу
    /// </summary>
    Task<OrderOperation> CreateSellOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitSellPrice, decimal quantity);

    /// <summary>
    /// Создание стоп ордера на продажу
    /// </summary>
    Task<OrderOperation> CreateSellStopOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity);
}
