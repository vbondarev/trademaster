﻿using TradeMaster.Core.Integrations.Binance;
using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Handlers;
using TradeMaster.Core.Trading.Models;

namespace TradeMaster.Core.Trading;

internal class Trader
{
    private readonly IBinanceProvider _binanceProvider;
    private readonly IBinanceTradeHandler _tradeHandler;
    private readonly RiskManagementHandler _riskManagementHandler;

    public Trader(IBinanceProvider binanceProvider, IBinanceTradeHandler tradeHandler, RiskManagementHandler riskManagementHandler)
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
    /// <param name="baseCoin">Базовая монета</param>
    /// <param name="quotedCoin">Котируемая монета</param>
    /// <param name="trend">Тренд</param>
    /// <param name="startAmount">Начальная сумма ордера</param>
    /// <param name="totalOrderCount">Количество монет, которыми будем торговать</param>
    public async Task<ResultStatisticModel> StartTrading(Coin baseCoin, Coin quotedCoin, Trend trend, decimal startAmount, int totalOrderCount)
    {
        switch (trend)
        {
            case Trend.Bear:
                while (totalOrderCount != 0)
                {
                    // необходимо рассчитать сумму ордера
                    var orderAmount = await _tradeHandler.CalculateOrderAmount(quotedCoin);
                    
                    // проверка выхода по стоп-лимиту
                    // если начальная сумма базовой монеты больше, чем общая сумма на спотовом кошельке, выходим по проебу
                    if (orderAmount > startAmount) break;

                    //получаем сумму заработанных средств
                    var profitAmount = orderAmount - startAmount;
                
                    //формируем историю изменений цены
                    //пока что возьмем за образец сведения двухчасовой давности, но в дальнейшем
                    //необходимо либо брать эту информацию из конфиг файлов, либо определять автоматически, что более приоритетно
                    var priceHistory = await _tradeHandler.GeneratePriceHistory(baseCoin, quotedCoin, Interval.QuarterHour, 8);

                    // формируем цену покупки
                    var buyPrice = _tradeHandler.CalculateBuyOrderPrice(trend, priceHistory);
                    var buyOrderOperation = await _tradeHandler.CreateBuyOrder(baseCoin, quotedCoin, buyPrice, orderAmount);
                    
                    // ожидание исполнения лимитного ордера на покупку котируемой монеты
                    // если ордер на покупку не исполнен в течение 10 минут, 
                    // отменяем его, и перезапускаем механизм
                    if (!buyOrderOperation.IsExecuted)
                    {
                        var cancelOrder = await _tradeHandler.CancelBuyOrder(baseCoin, quotedCoin);
                        if (cancelOrder.IsExecuted) continue;
                    }
                    
                    // необходимо рассчитать и создать стоп-лимит ордер на продажу
                    // стоп-лимит должен рассчитываться после покупки монеты
                    // стоп-лимит должен пересчитываться после каждого лимитного ордера на покупку
                    // на основе общего баланса и каждый предыдущий существующий должен отменяться
                
                    // если существует стоп-лимитный ордер на продажу, отменяем его
                    // проверяем существование стоп-лимитного ордера на продажу
                    var stopOrder =  await _tradeHandler.GetSellStopOrder(baseCoin, quotedCoin); 
                    if (stopOrder.IsExecuted)
                    {
                        var sellStopOrder = await _tradeHandler.CancelSellStopOrder(baseCoin, quotedCoin);
                        if (sellStopOrder.IsExecuted) continue;
                    }
                
                    // Расчет цены стоп-лимита на продажу
                    var stopLimitSellPrice = _riskManagementHandler.CalculateSellStopLimitOrder(trend, startAmount, buyOrderOperation.CoinCount, profitAmount, buyPrice);
                    var sellOrder = await _tradeHandler.CreateSellOrder(baseCoin, quotedCoin, stopLimitSellPrice, stopLimitSellPrice, buyOrderOperation.CoinCount);

                    //Формирование лимитного ордера на продажу
                    //Сформируем цену продажи котируемой монеты
                    var sellPrice = _tradeHandler.CalculateSellOrderPrice(baseCoin, quotedCoin,trend, priceHistory, buyPrice);
                    var quotedCoinCellResult = await _tradeHandler.CreateSellStopOrder(baseCoin, quotedCoin, sellPrice, buyOrderOperation.CoinCount);
                        
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

