﻿using TradeMaster.Binance;
using TradeMaster.Handlers;
using TradeMaster.Models;

namespace TradeMaster;

internal class Trader
{
    private readonly IBinanceConnector _binanceConnector;
    private readonly TradeHandler _tradeHandler;
    private readonly RiskManagementHandler _riskManagementHandler;

    public Trader(IBinanceConnector binanceConnector, TradeHandler tradeHandler, RiskManagementHandler riskManagementHandler)
    {
        _binanceConnector = binanceConnector;
        _tradeHandler = tradeHandler;
        _riskManagementHandler = riskManagementHandler;
    }
    
    //Разработать функционал по временным интервалам покупки
    //Допустим будем производить следующую покупку через 30 секунд после продажи
    //На текущий момент работаем только с биткойном

    /// <summary>
    /// Начало торговли
    /// </summary>
    public void StartTrading()
    {
        var trendDefiner = new TrendHandler();
        
        //Определяем тренд
        var currentTrend = trendDefiner.DefineTrend();

        switch (currentTrend)
        {
            case Trend.Bear:
                //необходимо рассчитать сумму ордера
                var orderAmount = _tradeHandler.CalculateOrderAmount(Coins.USDT);
                
                //формируем историю изменений цены
                //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
                //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
                var priceHistory = _tradeHandler.GeneratePriceHistory(Coins.BTC, Interval.QuarterHour, 8);
                
                //необходимо рассчитать и создать стоп-лимит ордер на продажу
                //стоп-лимит должен пересчитываться после каждого лимитного ордера на покупку
                //на основе общего баланса и каждый предыдущий существующий должен отменяться
                
                //если существует стоп-лимитный ордер на продажу, отменяем его
                //проверяем существование стоп-лимитного ордера на продажу
                var isCellStopLimitOrderExist = _binanceConnector.CellStopLimitOrderCheck(Coins.BTC);
                if (isCellStopLimitOrderExist)
                {
                    //если существует, удаляем его
                    var deleteCellStopLimitOrderResult = _binanceConnector.DeleteCellStopLimitOrder(Coins.BTC);
                }
                
                var stopLimitCellPrice = _riskManagementHandler.CalculateStopLimitCellOrder(Trend.Bear, orderAmount);
                var stopLimitCellAmount = _tradeHandler.CalculateOrderAmount(Coins.BTC);
                var stopLimitCellResult = _binanceConnector.CellCoins(Coins.USDT, OrderTypes.StopLimit,
                    stopLimitCellPrice, stopLimitCellAmount);
                
                //формируем цену покупки
                var buyPrice = _tradeHandler.CalculateBuyOrderPrice(Trend.Bear, priceHistory);
                //совершаем сделку через binanceConnector
                var result = _binanceConnector.BuyCoins(Coins.BTC, OrderTypes.Limit, buyPrice, orderAmount);
                
                break;
            case Trend.Bull:
                break;
            case Trend.Flat:
                break;
        }
    }
}
