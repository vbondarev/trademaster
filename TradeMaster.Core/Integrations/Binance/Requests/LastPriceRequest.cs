using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record LastPriceRequest : BaseRequest
{

    public LastPriceRequest(Coin baseCoin, Coin quotedCoin) : base(baseCoin, quotedCoin)
    {
    }
}
