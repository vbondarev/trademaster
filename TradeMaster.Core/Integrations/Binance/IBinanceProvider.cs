using TradeMaster.Core.Integrations.Binance.Dtos;
using TradeMaster.Core.Integrations.Binance.Responses;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Integrations.Binance;

public interface IBinanceProvider
{
    /// <summary>
    /// Запрашиваем статус системы
    /// </summary>
    /// <returns></returns>
    Task<BinanceStatus> GetSystemStatus();

    /// <summary>
    /// Создание ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<OrderInfo> CreateBuyLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity);

    /// <summary>
    /// Создание ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<OrderInfo> CreateSellLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity);
    
    /// <summary>
    /// Создание СТОП ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<OrderInfo> CreateSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitPrice, decimal quantity);

    /// <summary>
    /// Получить максимальную стоимость за определенный интервал
    /// </summary>
    /// <param name="baseCoin"></param>
    /// <param name="quotedCoin"></param>
    /// <param name="interval"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns>Максимальная стоимость торговой пары</returns>
    Task<decimal> GetMaxPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime);

    /// <summary>
    /// Получить минимальную стоимость за определенный интервал
    /// </summary>
    /// <param name="baseCoin"></param>
    /// <param name="quotedCoin"></param>
    /// <param name="interval"></param>
    /// <param name="startTime"></param>
    /// <param name="endTime"></param>
    /// <returns>Минимальная стоимость торговый пары</returns>
    Task<decimal> GetMinPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime);

    /// <summary>
    /// Получить актуальную стоимость и время последней стоимости монеты
    /// </summary>
    /// <returns></returns>
    Task<CoinPriceModel> GetLastPrice(Coin baseCoin, Coin quotedCoin);

    /// <summary>
    /// Получить общую сумму монет на спотовом аккаунте
    /// </summary>
    /// <returns></returns>
    Task<decimal> GetAccountBalance(Coin coin);

    /// <summary>
    /// Проверка существования стоп ордера на продажу
    /// </summary>
    Task<OrderInfo?> GetSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, long orderId);

    /// <summary>
    /// Удаление существующего лимитного ордера на покупку котируемой монеты
    /// </summary>
    Task<OrderInfo> CancelOrder(Coin baseCoin, Coin quotedCoin, long orderId);
}
