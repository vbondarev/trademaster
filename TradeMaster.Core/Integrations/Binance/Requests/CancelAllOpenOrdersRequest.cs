using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record CancelAllOpenOrdersRequest : BaseRequest
{
    public CancelAllOpenOrdersRequest(Coin baseCoin, Coin quotedCoin) 
        : base(baseCoin, quotedCoin) { }
}
