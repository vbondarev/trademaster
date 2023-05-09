using TradeMaster.Core.Enums;
using TradeMaster.Core.Models;
using BinanceInterval = Binance.Spot.Models.Interval;

namespace TradeMaster.Core.Binance.Requests;

public record CandlestickDataRequest : BaseRequest
{
    private readonly Interval _interval;
    private readonly DateTimeOffset _startTime;
    private readonly DateTimeOffset _endTime;

    public CandlestickDataRequest(Coin baseCoin, Coin quotedCoin, Interval interval, DateTimeOffset startTime, DateTimeOffset endTime) 
        : base(baseCoin, quotedCoin)
    {
        _interval = interval;
        _startTime = startTime;
        _endTime = endTime;
    }

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
    
