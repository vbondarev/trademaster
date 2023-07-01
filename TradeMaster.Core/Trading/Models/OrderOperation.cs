namespace TradeMaster.Core.Trading.Models;

public class OrderOperation
{
    public OrderOperation(long orderId, decimal coinCount, bool isExecuted)
    {
        this.OrderId = orderId;
        this.CoinCount = coinCount;
        this.IsExecuted = isExecuted;
    }

    public long OrderId { get; init; }
    public decimal CoinCount { get; init; }
    public bool IsExecuted { get; init; }
}
