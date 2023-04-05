namespace TradeMaster.Binance.Requests;

public record GetMaxPriceRequest
{
    private readonly Coins _firstCoin;
    private readonly Coins _second;
    private readonly DateTimeOffset _startTime;
    private readonly DateTimeOffset _endTime;

    public GetMaxPriceRequest(Coins firstCoin, Coins second, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        _firstCoin = firstCoin;
        _second = second;
        _startTime = startTime;
        _endTime = endTime;
    }

    public string CoinsPair => $"{_firstCoin}{_second}".ToUpperInvariant();
    
    public long StartTime => _startTime.ToUnixTimeMilliseconds();
    
    public long EndTime => _endTime.ToUnixTimeMilliseconds();
}
    
