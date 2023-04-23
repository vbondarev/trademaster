using Binance.Spot.Models;
using TradeMaster.Enums;
using TradeMaster.Exceptions;
using InternalOrderType = TradeMaster.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;

namespace TradeMaster.Binance.Requests;

public record NewOrderRequest : BaseRequest
{
    private readonly InternalOrderType _orderType;

    public NewOrderRequest(Coin baseCoin, Coin quotedCoin, InternalOrderType orderType, decimal quantity, decimal price) 
        : base(baseCoin, quotedCoin)
    
    {
        _orderType = orderType;
        Price = price;
        Quantity = quantity;
    }
    
    public Side Side { get; } = Side.BUY;

    public BinanceOrderType OrderType
    {
        get
        {
            return _orderType switch
            {
                InternalOrderType.Limit => BinanceOrderType.LIMIT,
                InternalOrderType.StopLossLimit => BinanceOrderType.STOP_LOSS_LIMIT,
                InternalOrderType.StopLoss => BinanceOrderType.STOP_LOSS,
                InternalOrderType.Market => BinanceOrderType.MARKET,
                _ => throw new BinanceUndefinedOrderType($"Тип ордера {_orderType} не определен")
            };
        }
    }

    public decimal Price { get; }
    
    public decimal Quantity { get; }
    
    /// <summary>
    /// Время действия ордера. Доступны следующие значения:
    /// GTC (Good Till Cancelled),
    /// IOC (Immediate or Cancel),
    /// FOK (Fill or Kill),
    /// GTD (Good Till Date),
    /// GTX (Good Till Crossing).
    /// </summary>
    /// <value>GTC (Good Till Cancelled)</value>
    public TimeInForce TimeInForce { get; } = TimeInForce.GTC;
}
