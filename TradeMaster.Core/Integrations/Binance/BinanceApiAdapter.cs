using Binance.Common;
using Binance.Spot;
using Microsoft.Extensions.Options;
using TradeMaster.Core.Integrations.Binance.Options;

namespace TradeMaster.Core.Integrations.Binance;

internal class BinanceApiAdapter
{
    private readonly HttpClient _httpClient;
    private readonly BinanceOptions _options;

    private SpotAccountTrade? _spotAccountTrade;
    private Wallet? _wallet;
    private Market? _market;

    public BinanceApiAdapter(IHttpClientFactory httpFactory, IOptions<BinanceOptions> options)
    {
        _httpClient = httpFactory.CreateClient("Binance");
        _options = options.Value;
    }
    public SpotAccountTrade GetSpotAccountTrade()
    {
        if (_spotAccountTrade != null) return _spotAccountTrade;

        var signature = new BinanceHmac(_options.SecretKey);
        
        _spotAccountTrade = new SpotAccountTrade(_httpClient, signature, apiKey: _options.ApiKey, baseUrl: _options.BaseUri);

        return _spotAccountTrade;
    }
    
    public Wallet GetWallet()
    {
        if (_wallet != null) return _wallet;
        _wallet = new Wallet(_httpClient);

        return _wallet;
    }
    
    public Market GetMarket()
    {
        if (_market != null) return _market;
        var signature = new BinanceHmac(_options.SecretKey);
        
        _market = new Market(_httpClient, signature, apiKey: _options.ApiKey, baseUrl: _options.BaseUri);

        return _market;
    }
}
