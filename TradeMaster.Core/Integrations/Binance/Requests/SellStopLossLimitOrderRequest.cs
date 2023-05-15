using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record SellStopLossLimitOrderRequest : SellOrderRequest
{
    public SellStopLossLimitOrderRequest(Coin baseCoin, Coin quotedCoin, decimal price, decimal stopLimitPrice, decimal quantity) 
        : base(baseCoin, quotedCoin, Enums.OrderType.STOP_LOSS_LIMIT, price, quantity)
    {
        StopLimitPrice = stopLimitPrice;
    }

    public decimal StopLimitPrice { get; private init; }
}
