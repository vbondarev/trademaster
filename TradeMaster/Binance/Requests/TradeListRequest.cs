using TradeMaster.Enums;
using InternalOrderType = TradeMaster.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;

namespace TradeMaster.Binance.Requests;

public record TradeListRequest : BaseRequest
{
    public TradeListRequest(Coin baseCoin, Coin quotedCoin, int orderId) 
        : base(baseCoin, quotedCoin)

    {
        OrderId = orderId;
    }
    
    public int OrderId { get; }
}
