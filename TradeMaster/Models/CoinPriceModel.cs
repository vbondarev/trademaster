namespace TradeMaster.Models;

public class CoinPriceModel
{
    public Coins Coin { get; set; }
    public DateTime Time { get; set; }
    public decimal Price { get; set; }
}