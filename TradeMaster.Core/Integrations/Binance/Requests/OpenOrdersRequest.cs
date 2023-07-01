using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record OpenOrdersRequest : BaseRequest
{
    public OpenOrdersRequest(Coin baseCoin, Coin quotedCoin) 
        : base(baseCoin, quotedCoin) { }
}
