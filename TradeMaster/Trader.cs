﻿using TradeMaster.Binance;
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
    public async Task StartTrading(Coin baseCoin, decimal startAmount)
    {
        var trendDefiner = new TrendHandler();
        
        //Определяем тренд
        var currentTrend = trendDefiner.DefineTrend();

        switch (currentTrend)
        {
            case Trend.Bear:
                //необходимо рассчитать сумму ордера
                var orderAmount = _tradeHandler.CalculateOrderAmount(Coin.USDT);
                
                //получаем сумму заработаных средств
                var profitAmount = orderAmount - startAmount;
                
                //формируем историю изменений цены
                //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
                //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
                var priceHistory = await _tradeHandler.GeneratePriceHistory(baseCoin,Coin.BTC, Interval.QuarterHour, 8);

                //формируем цену покупки
                var buyPrice = _tradeHandler.CalculateBuyOrderPrice(Trend.Bear, priceHistory);
                //совершаем сделку через binanceConnector
                var coinCount = _binanceProvider.BuyCoins(Coin.BTC, OrderTypes.Limit, buyPrice, orderAmount);
                
                //необходимо рассчитать и создать стоп-лимит ордер на продажу
                //стоп-лимит должен рассчитываться после покупки монеты
                //стоп-лимит должен пересчитываться после каждого лимитного ордера на покупку
                //на основе общего баланса и каждый предыдущий существующий должен отменяться
                
                //если существует стоп-лимитный ордер на продажу, отменяем его
                //проверяем существование стоп-лимитного ордера на продажу
                var isCellStopLimitOrderExist = _binanceProvider.CellStopLimitOrderCheck(Coin.BTC);
                if (isCellStopLimitOrderExist)
                {
                    //если существует, удаляем его
                    var deleteCellStopLimitOrderResult = _binanceProvider.DeleteCellStopLimitOrder(Coin.BTC);
                }
                
                var stopLimitCellPrice = _riskManagementHandler.CalculateStopLimitCellOrder(Trend.Bear, startAmount, coinCount, profitAmount, buyPrice);
                var stopLimitCellAmount = _tradeHandler.CalculateOrderAmount(Coin.BTC);
                var stopLimitCellResult = _binanceProvider.CellCoins(Coin.BTC, OrderTypes.StopLimit,
                    stopLimitCellPrice, stopLimitCellAmount);
                
                break;
            case Trend.Bull:
                break;
            case Trend.Flat:
                break;
        }
    }
}

