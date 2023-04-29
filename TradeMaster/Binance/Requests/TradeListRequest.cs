using TradeMaster.Enums;
using InternalOrderType = TradeMaster.Binance.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;

namespace TradeMaster.Binance.Requests;

public record TradeListRequest : BaseRequest
{
    public TradeListRequest(Coin baseCoin, Coin quotedCoin, long orderId) 
        : base(baseCoin, quotedCoin)

    {
        OrderId = orderId;
    }
    
    public long OrderId { get; }
}
