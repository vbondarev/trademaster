using TradeMaster.Enums;
using InternalOrderType = TradeMaster.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;

namespace TradeMaster.Binance.Requests;

public record QueryOrderRequest : BaseRequest
{
    public QueryOrderRequest(Coin baseCoin, Coin quotedCoin, long orderId) 
        : base(baseCoin, quotedCoin)

    {
        OrderId = orderId;
    }
    
    public long OrderId { get; }
}
