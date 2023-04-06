using System.Globalization;
using TradeMaster.Binance.Exceptions;
using TradeMaster.Binance.Requests;
using TradeMaster.Binance.Responses;
using TradeMaster.Models;

namespace TradeMaster.Binance;

public class BinanceProvider : IBinanceProvider
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

    public decimal BuyCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount)
    {
        throw new NotImplementedException();
    }

    public bool CellCoins(Coins coin, OrderTypes orderType, decimal price, decimal amount)
    {
        throw new NotImplementedException();
    }

    public async Task<decimal> GetMaxPrice(Coins baseCoin, Coins quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
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

    public async Task<decimal> GetMinPrice(Coins baseCoin, Coins quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
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

    public CoinPriceModel GetLastCoinPrice(Coins baseCoin, Coins quotedCoin)
    {
        throw new NotImplementedException();
    }

    public decimal GetTotalAmount(Coins coin)
    {
        throw new NotImplementedException();
    }

    public bool CellStopLimitOrderCheck(Coins coin)
    {
        throw new NotImplementedException();
    }

    public bool DeleteCellStopLimitOrder(Coins btc)
    {
        throw new NotImplementedException();
    }
}
