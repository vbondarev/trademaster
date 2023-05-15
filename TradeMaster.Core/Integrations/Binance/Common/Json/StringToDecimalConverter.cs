using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradeMaster.Core.Integrations.Binance.Common.Json;

internal class StringToDecimalConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is JsonTokenType.String)
        {
            var isParsed = decimal.TryParse(reader.GetString(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var value);
            if (isParsed) return value;
        }

        throw new JsonException($"Value {reader.GetString()} cannot be parsed to {nameof(Decimal)} type");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
    }
}
