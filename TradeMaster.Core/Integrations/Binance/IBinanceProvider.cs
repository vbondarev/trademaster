using TradeMaster.Core.Integrations.Binance.Enums;
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
    /// Метод для создания ордера на продажу
    /// </summary>
    Task<OrderResultModel> CreateBuyOrder(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal price, decimal quantity);

    /// <summary>
    /// Создание ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<OrderResultModel> CreateSellLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity);
    
    /// <summary>
    /// Создание СТОП ЛИМИТНОГО ордера на продажу
    /// </summary>
    Task<OrderResultModel> CreateSellStopLossLimitOrder(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitPrice, decimal quantity);

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
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    bool GetSellStopLimitOrder(Coin coin);

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    /// <returns></returns>
    bool DeleteSellStopLimitOrder(Coin btc);

    /// <summary>
    /// Удаление существующего лимитного ордера на покупку котируемой монеты
    /// </summary>
    Task<bool> DeleteBuyLimitOrder(Coin baseCoin, Coin quotedCoin);
}
