using Microsoft.Extensions.Logging;
using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Handlers;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Trading.Strategies;

internal class BinanceBearTrendStrategy : ITrendStrategy
{
    private readonly IBinanceProvider _binanceProvider;
    private readonly IBinanceTradeHandler _tradeHandler;
    private readonly RiskManagementHandler _riskManagementHandler;
    private readonly ILogger<BinanceBearTrendStrategy> _logger;

    public BinanceBearTrendStrategy(
        IBinanceProvider binanceProvider, 
        IBinanceTradeHandler tradeHandler,
        RiskManagementHandler riskManagementHandler,
        ILogger<BinanceBearTrendStrategy> logger)
    {
        _binanceProvider = binanceProvider;
        _tradeHandler = tradeHandler;
        _riskManagementHandler = riskManagementHandler;
        _logger = logger;
    }

    public async Task<TradeOperationResult> Run(StrategyParameter parameter)
    {
        var trend = Trend.Bear;
        var totalOrderCount = parameter.TotalOrderCount;
        while (totalOrderCount != 0)
        {
            _logger.LogInformation("Запуск медвежьей торговли на Binance");
            
            // необходимо рассчитать сумму ордера
            var orderAmount = await _tradeHandler.CalculateOrderAmount(parameter.QuotedCoin);

            // проверка выхода по стоп-лимиту
            // если начальная сумма базовой монеты больше, чем общая сумма на спотовом кошельке, выходим по проебу
            if (orderAmount > parameter.StartAmount) break;

            //получаем сумму заработанных средств
            var profitAmount = orderAmount - parameter.StartAmount;

            //формируем историю изменений цены
            //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
            //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
            var priceHistory = await _tradeHandler.GeneratePriceHistory(parameter.BaseCoin, parameter.QuotedCoin, Interval.QuarterHour, 8);

            // формируем цену покупки
            var buyPrice = _tradeHandler.CalculateBuyOrderPrice(trend, priceHistory);
            var buyOrderOperation = await _tradeHandler.CreateBuyOrder(parameter.BaseCoin, parameter.QuotedCoin, buyPrice, orderAmount);

            // ожидание исполнения лимитного ордера на покупку котируемой монеты
            // если ордер на покупку не исполнен в течение 10 минут, 
            // отменяем его, и перезапускаем механизм
            if (!buyOrderOperation.IsExecuted)
            {
                var cancelOrder = await _tradeHandler.CancelBuyOrder(parameter.BaseCoin, parameter.QuotedCoin);
                if (cancelOrder.IsExecuted) continue;
            }

            // необходимо рассчитать и создать стоп-лимит ордер на продажу
            // стоп-лимит должен рассчитываться после покупки монеты
            // стоп-лимит должен пересчитываться после каждого лимитного ордера на покупку
            // на основе общего баланса и каждый предыдущий существующий должен отменяться

            // если существует стоп-лимитный ордер на продажу, отменяем его
            // проверяем существование стоп-лимитного ордера на продажу
            var stopOrder = await _tradeHandler.GetSellStopOrder(parameter.BaseCoin, parameter.QuotedCoin);
            if (stopOrder.IsExecuted)
            {
                var sellStopOrder = await _tradeHandler.CancelSellStopOrder(parameter.BaseCoin, parameter.QuotedCoin);
                if (sellStopOrder.IsExecuted) continue;
            }

            // Расчет цены стоп-лимита на продажу
            var stopLimitSellPrice = _riskManagementHandler.CalculateSellStopLimitOrder(trend, parameter.StartAmount,
                buyOrderOperation.CoinCount, profitAmount, buyPrice);
            var sellOrder = await _tradeHandler.CreateSellOrder(parameter.BaseCoin, parameter.QuotedCoin, stopLimitSellPrice,
                stopLimitSellPrice, buyOrderOperation.CoinCount);

            //Формирование лимитного ордера на продажу
            //Сформируем цену продажи котируемой монеты
            var sellPrice = _tradeHandler.CalculateSellOrderPrice(parameter.BaseCoin, parameter.QuotedCoin, trend, priceHistory, buyPrice);
            var quotedCoinCellResult =
                await _tradeHandler.CreateSellStopOrder(parameter.BaseCoin, parameter.QuotedCoin, sellPrice, buyOrderOperation.CoinCount);

            //Ожидаем исполнения лимитного ордера на продажу
            if (quotedCoinCellResult.IsExecuted) totalOrderCount--;
            else
            {
                //если ордер лимитный ордер на продажу не исполнен, нельзя переходить на следующую итерацию, 
                //так как мы не продали котируемую монету
                //Здесь надо продумать алгоритм действий в случае если мы не продали котируемую монету
                //Нужно перерасчитать сумму лимитного ордера на продажу
                break;
            }
        }

        var baseCoinTotalAmount = await _binanceProvider.GetAccountBalance(parameter.BaseCoin);
        var result = new TradeOperationResult
        {
            BaseCoinTotal = baseCoinTotalAmount,
            QuotedCoinTotal = await _binanceProvider.GetAccountBalance(parameter.QuotedCoin),
            ProfitAmount = baseCoinTotalAmount - parameter.StartAmount
        };

        return result;
    }
}
