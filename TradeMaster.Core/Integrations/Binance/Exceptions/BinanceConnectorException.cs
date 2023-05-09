namespace TradeMaster.Core.Integrations.Binance.Exceptions;

public class BinanceConnectorException : Exception
{
    public BinanceConnectorException(string? message) : base(message)
    {
    }
}
