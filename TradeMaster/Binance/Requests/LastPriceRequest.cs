using TradeMaster.Enums;

namespace TradeMaster.Binance.Requests;

public record LastPriceRequest : BaseRequest
{

    public LastPriceRequest(Coin baseCoin, Coin quotedCoin) : base(baseCoin, quotedCoin)
    {
    }
}
