﻿using System.Globalization;
using TradeMaster.Binance.Requests;
using TradeMaster.Binance.Responses;
using TradeMaster.Enums;
using TradeMaster.Exceptions;
using TradeMaster.Models;

namespace TradeMaster.Binance;

internal class BinanceProvider : IBinanceProvider
{
    private readonly IBinanceConnector _connector;

    public BinanceProvider(IBinanceConnector connector)
    {
        _connector = connector;
    }

    public async Task<BinanceStatus> GetSystemStatus()
    {
        var status = await _connector.GetSystemStatus();

        return status.Status switch
        {
            0 => BinanceStatus.Normal,
            1 => BinanceStatus.Maintenance,
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };
    }

    /// <summary>
    /// Метод покупки монеты. В данном методе необходимо реализовать механизм, который вернет успех.
    /// В случае, если в течение 10 минут ордер на покупку не будет исполнен, необходимо вернуть false в свойстве Success.
    /// Важно мониторить результат выполнения ордера, и как только он будет исполнен, вернуть true в свойстве Success.
    /// Если через 10 минут ордер не исполнен, Антон перезапустит механизм покупки.
    /// </summary>
    public async Task<OrderResultModel> BuyCoins(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal price, decimal quantity)
    {
        var request = new NewOrderRequest(baseCoin, quotedCoin, orderType, price, quantity);
        var response = await _connector.CreateNewOrder(request);

        return new OrderResultModel();
    }

    public OrderResultModel CellCoins(Coin coin, OrderType orderType, decimal price, decimal amount)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Удаление существующего лимитного ордера на покупку котируемой монеты
    /// </summary>
    /// <param name="quotedCoin"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public bool DeleteBuyLimitOrder(Coin quotedCoin)
    {
        throw new NotImplementedException();
    }
    

    public async Task<decimal> GetMaxPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var request = new CandlestickDataRequest(baseCoin, quotedCoin, interval, startTime, endTime);
        var candlesticks = await _connector.GetCandlestickData(request);
        var highestPrices = new List<decimal>(candlesticks.Length); 
        const int HighestPriceIndex = 2;
        
        foreach (var candlestick in candlesticks)
        {
            if (!decimal.TryParse(candlestick[HighestPriceIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out var highPrice))
            {
                throw new BinanceHighPriceException("Не удалось получить максимальную цену свечи");
            }

            highestPrices.Add(highPrice);
        }

        return highestPrices.Max();
    }

    public async Task<decimal> GetMinPrice(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        var request = new CandlestickDataRequest(baseCoin, quotedCoin, interval, startTime, endTime);
        var candlesticks = await _connector.GetCandlestickData(request);
        var lowestPrices = new List<decimal>(candlesticks.Length); 
        const int LowestPriceIndex = 3;
        
        foreach (var candlestick in candlesticks)
        {
            if (!decimal.TryParse(candlestick[LowestPriceIndex], NumberStyles.Any, CultureInfo.InvariantCulture, out var highPrice))
            {
                throw new BinanceHighPriceException("Не удалось получить минимальную цену свечи");
            }

            lowestPrices.Add(highPrice);
        }

        return lowestPrices.Min();
    }

    public async Task<CoinPriceModel> GetLastPrice(Coin baseCoin, Coin quotedCoin)
    {
        var request = new LastPriceRequest(baseCoin, quotedCoin);
        var response = await _connector.GetSymbolPriceTicker(request);

        if (decimal.TryParse(response.Price, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
        {
            return new CoinPriceModel(baseCoin, quotedCoin) { Price = price, Time = DateTimeOffset.Now };    
        }

        throw new InvalidCastException($"Не удалось преобразовать актуальную цену {response.Price} в число");
    }

    public async Task<decimal> GetTotalAmount(Coin coin)
    {
        var response = await _connector.GetAccountInformation();
        var balance = response.Balances.Single(balance => balance.Asset == coin.ToString());
        
        if (decimal.TryParse(balance.Free, NumberStyles.Any, CultureInfo.InvariantCulture, out var amount))
        {
            return amount;    
        }

        throw new InvalidCastException($"Не удалось преобразовать баланс кошелька {balance.Free} в число");
    }

    public bool CellStopLimitOrderCheck(Coin coin)
    {
        throw new NotImplementedException();
    }

    public bool DeleteCellStopLimitOrder(Coin btc)
    {
        throw new NotImplementedException();
    }
}
