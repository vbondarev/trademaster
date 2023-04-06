using TradeMaster.Models;
using BinanceInterval = Binance.Spot.Models.Interval;

namespace TradeMaster.Binance.Requests;

public record CandlestickDataRequest
{
    private readonly Coins _baseCoin;
    private readonly Coins _quotedCoin;
    private readonly Interval _interval;
    private readonly DateTimeOffset _startTime;
    private readonly DateTimeOffset _endTime;

    public CandlestickDataRequest(Coins baseCoin, Coins quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        _baseCoin = baseCoin;
        _quotedCoin = quotedCoin;
        _interval = interval;
        _startTime = startTime;
        _endTime = endTime;
    }

    public string CoinsPair => $"{_baseCoin}{_quotedCoin}".ToUpperInvariant();
    
    public long StartTime => _startTime.ToUnixTimeMilliseconds();
    
    public long EndTime => _endTime.ToUnixTimeMilliseconds();

    public BinanceInterval Interval => _interval switch
    {
        Models.Interval.Minute => BinanceInterval.ONE_MINUTE,
        Models.Interval.QuarterHour => BinanceInterval.FIFTEEN_MINUTE,
        Models.Interval.HalfHour => BinanceInterval.THIRTY_MINUTE,
        Models.Interval.Hour => BinanceInterval.ONE_HOUR,
        Models.Interval.TwoHour => BinanceInterval.TWO_HOUR,
        Models.Interval.FourHour => BinanceInterval.FOUR_HOUR,
        Models.Interval.EightHour => BinanceInterval.EIGTH_HOUR,
        Models.Interval.Day => BinanceInterval.ONE_DAY,
        Models.Interval.Week => BinanceInterval.ONE_WEEK,
        Models.Interval.Month => BinanceInterval.ONE_MONTH,
        _ => throw new ArgumentException($"Интервал {_interval} не нашел отражения в {nameof(BinanceInterval)}")
    };
}
    
