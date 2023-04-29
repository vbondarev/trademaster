using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeMaster.Binance.Common;

internal static class Json
{
    public static T? Deserialize<T>(string obj)
    {
        var enumConverter = new JsonStringEnumConverter();
        var options = new JsonSerializerOptions();
        options.Converters.Add(enumConverter);
        
        return System.Text.Json.JsonSerializer.Deserialize<T>(obj, options);
    }
}
