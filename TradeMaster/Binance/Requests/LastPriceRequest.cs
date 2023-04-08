using TradeMaster.Enums;

namespace TradeMaster.Binance.Requests;

public record LastPriceRequest
{
    private readonly Coin _baseCoin;
    private readonly Coin _quotedCoin;

    public LastPriceRequest(Coin baseCoin, Coin quotedCoin)
    {
        _baseCoin = baseCoin;
        _quotedCoin = quotedCoin;
    }
    
    public string CoinsPair => $"{_baseCoin}{_quotedCoin}".ToUpperInvariant();
}
