namespace TradeMaster.Binance.Exceptions;

public class BinanceHighPriceException : Exception
{
    public BinanceHighPriceException(string message) : base(message)
    {
    }
}
