using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Common.Json;

internal static class Json
{
    public static T? Deserialize<T>(string obj)
    {
        var enumConverter = new JsonStringEnumConverter();
        var stringToDecimalConverter = new StringToDecimalConverter();
        var options = new JsonSerializerOptions();
        
        options.Converters.Add(enumConverter);
        options.Converters.Add(stringToDecimalConverter);
        
        return JsonSerializer.Deserialize<T>(obj, options);
    }
}
