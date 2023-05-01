using Binance.Spot.Models;
using TradeMaster.Enums;
using TradeMaster.Exceptions;
using InternalOrderType = TradeMaster.Binance.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;

namespace TradeMaster.Binance.Requests;

public record BuyOrderRequest : BaseRequest
{
    private readonly InternalOrderType _orderType;

    public BuyOrderRequest(Coin baseCoin, Coin quotedCoin, InternalOrderType orderType, decimal quantity, decimal price) 
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
                InternalOrderType.LIMIT => BinanceOrderType.LIMIT,
                InternalOrderType.STOP_LOSS_LIMIT => BinanceOrderType.STOP_LOSS_LIMIT,
                InternalOrderType.STOP_LOSS => BinanceOrderType.STOP_LOSS,
                InternalOrderType.MARKET => BinanceOrderType.MARKET,
                _ => throw new BinanceProviderException($"Тип ордера {_orderType} не определен")
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
