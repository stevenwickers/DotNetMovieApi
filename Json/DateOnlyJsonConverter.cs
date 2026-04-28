using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DotNetMovieApi.Json;

public sealed class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private static readonly string[] SupportedFormats =
    [
        "yyyy-MM-dd",
        "MM/dd/yyyy",
        "M/d/yyyy"
    ];

    public override DateOnly Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var value = reader.GetString();

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Date value cannot be empty.");
        }

        if (DateOnly.TryParseExact(
                value,
                SupportedFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date))
        {
            return date;
        }

        throw new JsonException("Date value must use yyyy-MM-dd or MM/dd/yyyy format.");
    }

    public override void Write(
        Utf8JsonWriter writer,
        DateOnly value,
        JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
    }
}
