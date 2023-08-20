using Microsoft.Extensions.Logging;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Strategies;

namespace TradeMaster.Core.Trading;

internal class Trader : ITrader
{
    private readonly Dictionary<Trend, ITrendStrategy> _strategies;
    private readonly IBinanceProvider _binanceProvider;
    private readonly ILogger<Trader> _logger;

    public Trader(Dictionary<Trend, ITrendStrategy> strategies, IBinanceProvider binanceProvider, ILogger<Trader> logger)
    {
        _strategies = strategies;
        _binanceProvider = binanceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Запуск торговли
    /// </summary>
    /// <remarks>По хорошему сюда нужно передать параметры торговли</remarks>
    public async Task StartTrading()
    {
        _logger.LogInformation("n,m,hjgjhfkhjgf");
        _logger.LogDebug("n,m,hjgjhfkhjgf");
        var baseCoin = Coin.BTC;
        var quotedCoin = Coin.USDT;
        var lastPrice = (await _binanceProvider.GetLastPrice(baseCoin, quotedCoin)).Price;
        var strategyParameters = new StrategyParameter
        {
            BaseCoin = Coin.BTC, QuotedCoin = Coin.USDT, TotalOrderCount = 1, StartAmount = lastPrice
        };

        var bearStrategy = _strategies[Trend.Bear];
        var result = await bearStrategy.Run(strategyParameters);
    }
}

