namespace TradeMaster.Core.Trading;

public interface ITrader
{
    /// <summary>
    /// Запуск торговли
    /// </summary>
    Task StartTrading();
}
