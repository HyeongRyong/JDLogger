using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Formatter;
using JDLoggerV1.Formatting.Models;
using System.Text.Json;

namespace JDLoggerV1.Formatting.Formatters;

/// <summary>
/// JSON 형식의 로그 포맷터
/// </summary>
public class JsonLogFormatter : ILogFormatter<ILogModel>
{
    private static readonly Lazy<JsonLogFormatter> instance = new(() => new JsonLogFormatter());
    public static JsonLogFormatter Instance => instance.Value;

    public string GetHeader => null;

    private static readonly JsonSerializerOptions _options = new()
    {
        WriteIndented = true
    };

    public string Format(LogFormatData data)
    {
        var obj = data.Fields.ToDictionary(
            field => field.Name,
            field => field.Value
        );

        return JsonSerializer.Serialize(obj, _options);
    }
}