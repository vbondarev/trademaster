using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public abstract record BaseRequest
{
    private readonly Coin _baseCoin;
    private readonly Coin _quotedCoin;

    protected BaseRequest(Coin baseCoin, Coin quotedCoin)
    {
        _baseCoin = baseCoin;
        _quotedCoin = quotedCoin;
    }

    public string CoinsPair => $"{_baseCoin}{_quotedCoin}".ToUpperInvariant();
}
