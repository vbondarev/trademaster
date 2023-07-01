using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record BuyLimitOrderRequest : BuyOrderRequest
{
    public BuyLimitOrderRequest(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity ) 
        : base(baseCoin, quotedCoin, Enums.OrderType.LIMIT, price, quantity)
    {
    }
}
