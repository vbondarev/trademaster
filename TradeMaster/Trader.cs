using TradeMaster.Binance;
using TradeMaster.Binance.Enums;
using TradeMaster.Enums;
using TradeMaster.Handlers;
using TradeMaster.Models;

namespace TradeMaster;

internal class Trader
{
    private readonly IBinanceProvider _binanceProvider;
    private readonly TradeHandler _tradeHandler;
    private readonly RiskManagementHandler _riskManagementHandler;

    public Trader(IBinanceProvider binanceProvider, TradeHandler tradeHandler, RiskManagementHandler riskManagementHandler)
    {
        _binanceProvider = binanceProvider;
        _tradeHandler = tradeHandler;
        _riskManagementHandler = riskManagementHandler;
    }
    
    //Разработать функционал по временным интервалам покупки
    //Допустим будем производить следующую покупку через 30 секунд после продажи
    //На текущий момент работаем только с биткойном

    /// <summary>
    /// Начало торговли
    /// </summary>
    public async Task<ResultStatisticModel> StartTrading(Coin baseCoin, Coin quotedCoin, decimal startAmount, int totalOrderCount)
    {
        var trendDefiner = new TrendHandler();
        
        //Определяем тренд
        var currentTrend = trendDefiner.DefineTrend();

        switch (currentTrend)
        {
            case Trend.Bear:
                while (totalOrderCount != 0)
                {
                    //необходимо рассчитать сумму ордера
                    var orderQuantity = await _tradeHandler.CalculateOrderAmount(Coin.USDT);
                    
                    //проверка выхода по стоп-лимиту
                    //если начальная сумма базовой монеты больше, чем общая сумма на спотовом кошельке, выходим по проебу
                    if (orderQuantity > startAmount) break;

                    //получаем сумму заработаных средств
                    var profitAmount = orderQuantity - startAmount;
                
                    //формируем историю изменений цены
                    //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
                    //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
                    var priceHistory = await _tradeHandler.GeneratePriceHistory(baseCoin, quotedCoin, Interval.QuarterHour, 8);

                    //формируем цену покупки
                    var buyPrice = _tradeHandler.CalculateBuyOrderPrice(Trend.Bear, priceHistory);
                    var quotedCoinBuyResult = await _binanceProvider.BuyCoins(baseCoin, quotedCoin, OrderType.LIMIT, buyPrice, orderQuantity);
                    
                    //ожидание исполнения лимитного ордера на покупку котируемой монеты
                    //Если ордер на покупку не исполнен в течение 10 минут, 
                    //отменяем его, и перезапускаем механизм
                    if (!quotedCoinBuyResult.Success)
                    {
                        var deleteCellStopLimitOrderResult = _binanceProvider.DeleteBuyLimitOrder(quotedCoin);
                        if (deleteCellStopLimitOrderResult) continue;
                    }
                    
                    //необходимо рассчитать и создать стоп-лимит ордер на продажу
                    //стоп-лимит должен рассчитываться после покупки монеты
                    //стоп-лимит должен пересчитываться после каждого лимитного ордера на покупку
                    //на основе общего баланса и каждый предыдущий существующий должен отменяться
                
                    //если существует стоп-лимитный ордер на продажу, отменяем его
                    //проверяем существование стоп-лимитного ордера на продажу
                    var isCellStopLimitOrderExist = _binanceProvider.CellStopLimitOrderCheck(quotedCoin);
                    if (isCellStopLimitOrderExist)
                    {
                        //если существует, удаляем его
                        var deleteCellStopLimitOrderResult = _binanceProvider.DeleteCellStopLimitOrder(quotedCoin);
                    }
                
                    //Рассчет цены стоп-лимита на продажу
                    var stopLimitCellPrice = _riskManagementHandler.CalculateStopLimitCellOrder(Trend.Bear, startAmount, quotedCoinBuyResult.CoinCount, profitAmount, buyPrice);
                    var stopLimitCellCount = _binanceProvider.SellCoins(baseCoin, quotedCoin, OrderType.STOP_LOSS_LIMIT,
                        stopLimitCellPrice, quotedCoinBuyResult.CoinCount);

                    //Формирование лимитного ордера на продажу
                    //Сформируем цену продажи котируемой монеты
                    var cellPrice = _tradeHandler.CalculateCellOrderPrice(baseCoin, quotedCoin,Trend.Bear, priceHistory, buyPrice);
                    var quotedCoinCellResult = await 
                        _binanceProvider.SellCoins(baseCoin, quotedCoin, OrderType.LIMIT, cellPrice, quotedCoinBuyResult.CoinCount);
                        
                    //Ожидаем исполнения лимитного ордера на продажу
                    if (quotedCoinCellResult.Success) totalOrderCount--;
                    else
                    {
                        //если ордер лимитный ордер на продажу не исполнен, нельзя переходить на следующую итерацию, 
                        //так как мы не продали котируемую монету
                        //Здесь надо продумать алгоритм действий в случае если мы не продали котируемую монету
                        //Нужно перерасчитать сумму лимитного ордера на продажу
                        break;
                    }
                }
                
                break;
            case Trend.Bull:
                break;
            case Trend.Flat:
                break;
        }

        var baseCoinTotalAmount = await _binanceProvider.GetAccountBalance(baseCoin);
        var result = new ResultStatisticModel
        {
            BaseCoinTotal = baseCoinTotalAmount,
            QuotedCoinTotal = await _binanceProvider.GetAccountBalance(quotedCoin),
            ProfitAmount = baseCoinTotalAmount - startAmount
        };

        return result;
    }
}

