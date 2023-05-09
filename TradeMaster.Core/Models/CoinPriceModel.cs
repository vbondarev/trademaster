using TradeMaster.Core.Enums;

namespace TradeMaster.Core.Models;

public record CoinPriceModel
{
    public CoinPriceModel(Coin baseCoin, Coin quotedCoin)
    {
        CoinsPair = $"{baseCoin}{quotedCoin}".ToUpperInvariant();
    }

    public string CoinsPair { get; private set; } = null!;
    
    public DateTimeOffset Time { get; set; }
    
    public decimal Price { get; set; }
}
