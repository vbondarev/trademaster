using Binance.Spot.Models;
using TradeMaster.Core.Enums;
using TradeMaster.Core.Exceptions;
using InternalOrderType = TradeMaster.Core.Binance.Enums.OrderType;
using BinanceOrderType = Binance.Spot.Models.OrderType;
using OrderType = TradeMaster.Core.Binance.Enums.OrderType;

namespace TradeMaster.Core.Binance.Requests;

public record BuyOrderRequest : BaseRequest
{
    private readonly OrderType _orderType;

    public BuyOrderRequest(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal quantity, decimal price) 
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
                Enums.OrderType.LIMIT => BinanceOrderType.LIMIT,
                Enums.OrderType.STOP_LOSS_LIMIT => BinanceOrderType.STOP_LOSS_LIMIT,
                Enums.OrderType.STOP_LOSS => BinanceOrderType.STOP_LOSS,
                Enums.OrderType.MARKET => BinanceOrderType.MARKET,
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
