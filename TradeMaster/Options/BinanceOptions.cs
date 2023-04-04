namespace TradeMaster.Options;

public record BinanceOptions
{
    public string SpotUri { get; set; } = null!;
    
    public string ApiKey { get; set; } = null!;
}
