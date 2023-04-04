using TradeMaster.Binance.Responses;
using TradeMaster.Models;

namespace TradeMaster.Binance;

public interface IBinanceConnector
{
    /// <summary>
    /// Запрашиваем статус системы
    /// </summary>
    /// <returns></returns>
    Task<SystemStatusResponse> GetSystemStatus();

    /// <summary>
    /// Метод для покупки криптовалюты на Binance
    /// </summary>
    bool BuyCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount);

    /// <summary>
    /// Метод для продажи криптовалюты на Binance
    /// </summary>
    bool CellCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount);

    /// <summary>
    /// Получить максимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    decimal GetMaxPrice(Coins coin, DateTime startDateTime, DateTime endDateTime);

    /// <summary>
    /// Получить минимальную стоимость в определенном интервале
    /// </summary>
    /// <returns></returns>
    decimal GetMinPrice(Coins coin, DateTime startDateTime, DateTime endDateTime);

    /// <summary>
    /// Получить актуальную стоимость и время последней стоимости монеты
    /// </summary>
    /// <returns></returns>
    CoinPriceModel GetLastCoinPrice(Coins coin);

    /// <summary>
    /// Получить общую сумму монет на спотовом аккаунте
    /// </summary>
    /// <returns></returns>
    decimal GetTotalAmount(Coins coin);

    /// <summary>
    /// Проверка существования стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="coin"></param>
    /// <returns></returns>
    bool CellStopLimitOrderCheck(Coins coin);

    /// <summary>
    /// Удаление существующего стоп-лимитного ордера на продажу
    /// </summary>
    /// <param name="btc"></param>
    /// <returns></returns>
    bool DeleteCellStopLimitOrder(Coins btc);
}
