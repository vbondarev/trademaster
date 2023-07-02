using Microsoft.Extensions.Logging;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Core.Trading;

internal class Trader : ITrader
{
    private readonly Dictionary<Trend, ITrendStrategy> _strategies;
    private readonly ILogger<Trader> _logger;

    public Trader(Dictionary<Trend, ITrendStrategy> strategies, ILogger<Trader> logger)
    {
        _strategies = strategies;
        _logger = logger;
    }

    /// <summary>
    /// Запуск торговли
    /// </summary>
    /// <param name="parameters">Список стратегий, которые нужно запустить</param>
    /// <returns></returns>
    public async Task StartTrading(Dictionary<Trend, TradeParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (_strategies.TryGetValue(parameter.Key, out var strategy))
            {
                var tradeOperation = await strategy.Run(parameter.Value);
            }
            else
            {
                _logger.LogWarning("Стратегия торговли для \"{Trend} \" не найдена", parameter.Key);
            }
        }
    }
}

