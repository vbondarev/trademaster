using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record SellLimitOrderRequest : SellOrderRequest
{
    public SellLimitOrderRequest(Coin baseCoin, Coin quotedCoin, decimal price, decimal quantity) 
        : base(baseCoin, quotedCoin, Enums.OrderType.LIMIT, price, quantity)
    {
    }
}
