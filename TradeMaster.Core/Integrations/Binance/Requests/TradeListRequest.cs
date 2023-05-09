using TradeMaster.Core.Trading.Enums;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public record TradeListRequest : BaseRequest
{
    public TradeListRequest(Coin baseCoin, Coin quotedCoin, long orderId) 
        : base(baseCoin, quotedCoin)

    {
        OrderId = orderId;
    }
    
    public long OrderId { get; }
}
