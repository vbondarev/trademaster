using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Core.Trading;

public interface ITrader
{
    /// <summary>
    /// Запуск торговли
    /// </summary>
    Task StartTrading(Dictionary<Trend, TradeParameter> parameters);
}
