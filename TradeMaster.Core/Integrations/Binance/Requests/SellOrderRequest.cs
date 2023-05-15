using Binance.Spot.Models;
using TradeMaster.Core.Integrations.Binance.Exceptions;
using TradeMaster.Core.Trading.Enums;
using BinanceOrderType = Binance.Spot.Models.OrderType;
using OrderType = TradeMaster.Core.Integrations.Binance.Enums.OrderType;

namespace TradeMaster.Core.Integrations.Binance.Requests;

public abstract record SellOrderRequest : BaseRequest
{
    private readonly OrderType _orderType;

    protected SellOrderRequest(Coin baseCoin, Coin quotedCoin, OrderType orderType, decimal price, decimal quantity) 
        : base(baseCoin, quotedCoin)
    
    {
        _orderType = orderType;
        Price = price;
        Quantity = quantity;
    }
    
    public string ClientOrderId { get; } = Guid.NewGuid().ToString();
    
    public Side Side { get; } = Side.SELL;

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
