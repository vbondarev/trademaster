using TradeMaster.Core.Enums;

namespace TradeMaster.Core.Binance.Requests;

public record LastPriceRequest : BaseRequest
{

    public LastPriceRequest(Coin baseCoin, Coin quotedCoin) : base(baseCoin, quotedCoin)
    {
    }
}
