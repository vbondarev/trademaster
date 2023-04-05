namespace TradeMaster.Binance.Requests;

public record GetMaxPriceRequest
{
    private readonly Coins _baseCoin;
    private readonly Coins _quotedCoin;
    private readonly DateTimeOffset _startTime;
    private readonly DateTimeOffset _endTime;

    public GetMaxPriceRequest(Coins baseCoin, Coins quotedCoin, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        _baseCoin = baseCoin;
        _quotedCoin = quotedCoin;
        _startTime = startTime;
        _endTime = endTime;
    }

    public string CoinsPair => $"{_baseCoin}{_quotedCoin}".ToUpperInvariant();
    
    public long StartTime => _startTime.ToUnixTimeMilliseconds();
    
    public long EndTime => _endTime.ToUnixTimeMilliseconds();
}
    
