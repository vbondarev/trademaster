using TradeMaster.Binance.Responses;
using TradeMaster.Enums;
using TradeMaster.Models;

namespace TradeMaster.Binance;

public interface IBinanceProvider
{
    /// <summary>
    /// Запрашиваем статус системы
    /// </summary>
    /// <returns></returns>
    Task<BinanceStatus> GetSystemStatus();

    /// <summary>
    /// Метод для покупки криптовалюты на Binance
    /// </summary>
    OrderResultModel BuyCoins(Coin coin, OrderTypes orderType, decimal price, decimal amount);

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    OrderResultModel CellCoins(Coin coin, OrderTypes orderType, decimal price, decimal amount);

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
    Task<decimal> GetTotalAmount(Coin coin);

    /// <summary>
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    bool CellStopLimitOrderCheck(Coin coin);

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    /// <returns></returns>
    bool DeleteCellStopLimitOrder(Coin btc);

    /// <summary>
    /// Удаление существующего лимитного ордера на покупку котируемой монеты
    /// </summary>
    /// <param name="quotedCoin"></param>
    /// <returns></returns>
    bool DeleteBuyLimitOrder(Coin quotedCoin);
}
