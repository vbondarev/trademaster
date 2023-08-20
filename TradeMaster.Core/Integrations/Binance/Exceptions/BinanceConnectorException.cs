namespace TradeMaster.Core.Integrations.Binance.Exceptions;

public class BinanceConnectorException : Exception
{
    public int ErrorCode { get; }
    public string ErrorMessage { get; }

    /// <summary>
    /// Handle Binance layer exceptions 
    /// </summary>
    /// <param name="errorCode">Error code</param>
    /// <param name="errorMessage">Error message</param>
    /// <param name="exception"></param>
    public BinanceConnectorException(int errorCode, string errorMessage, Exception exception)
        : base($"Code: {errorCode}\nMessage: {errorMessage}", exception)
    {
        ErrorCode = errorCode;
        ErrorMessage = errorMessage;
    }
}
