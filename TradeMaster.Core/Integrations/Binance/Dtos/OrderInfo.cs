using TradeMaster.Core.Integrations.Binance.Enums;

namespace TradeMaster.Core.Integrations.Binance.Dtos;

public class OrderInfo
{
    public OrderInfo(long id, decimal executedQty, OrderSide side, OrderStatus status, OrderType type)
    {
        Id = id;
        ExecutedQty = executedQty;
        Side = side;
        Type = type;
        Status = status;
    }

    public long Id { get; init; }    
    
    public decimal ExecutedQty { get; init; }
    
    public OrderSide Side { get; init; }    
    
    public OrderType Type { get; init; }
    
    public OrderStatus Status { get; init; }
}
