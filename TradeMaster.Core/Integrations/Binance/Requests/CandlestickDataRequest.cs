using TradeMaster.Core.Trading.Enums;
using TradeMaster.Core.Trading.Models;
using BinanceInterval = Binance.Spot.Models.Interval;

namespace TradeMaster.Core.Integrations.Binance.Requests;

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
        Trading.Models.Interval.Minute => BinanceInterval.ONE_MINUTE,
        Trading.Models.Interval.QuarterHour => BinanceInterval.FIFTEEN_MINUTE,
        Trading.Models.Interval.HalfHour => BinanceInterval.THIRTY_MINUTE,
        Trading.Models.Interval.Hour => BinanceInterval.ONE_HOUR,
        Trading.Models.Interval.TwoHour => BinanceInterval.TWO_HOUR,
        Trading.Models.Interval.FourHour => BinanceInterval.FOUR_HOUR,
        Trading.Models.Interval.EightHour => BinanceInterval.EIGTH_HOUR,
        Trading.Models.Interval.Day => BinanceInterval.ONE_DAY,
        Trading.Models.Interval.Week => BinanceInterval.ONE_WEEK,
        Trading.Models.Interval.Month => BinanceInterval.ONE_MONTH,
        _ => throw new ArgumentException($"Интервал {_interval} не нашел отражения в {nameof(BinanceInterval)}")
    };
}
    
