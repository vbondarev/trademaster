namespace TradeMaster.Exceptions;

public class BinanceConnectorException : Exception
{
    public BinanceConnectorException(string? message) : base(message)
    {
    }
}
