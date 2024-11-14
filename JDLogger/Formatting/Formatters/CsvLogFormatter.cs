using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Models;

namespace JDLoggerV1.Formatting.Formatter;

/// <summary>
/// CSV 형식의 로그 포맷터
/// </summary>
public class CsvLogFormatter(string header = null) : ILogFormatter<ILogModel>
{
    private static readonly Lazy<CsvLogFormatter> instance = new(() => new CsvLogFormatter());
    public static CsvLogFormatter Instance => instance.Value;

    private readonly string _header = header;

    public string GetHeader => _header;

    public string Format(LogFormatData data)
    {
        var values = data.Fields.Select(field =>
        {
            if (field.Value == null)
                return string.Empty;

            // 문자열 필드는 이스케이프 처리
            if (field.Type == typeof(string))
            {
                return $"\"{EscapeCsvField(field.Value.ToString())}\"";
            }

            // 날짜 형식 지정
            if (field.Type == typeof(DateTime))
            {
                return ((DateTime)field.Value).ToString("yyyy-MM-dd HH:mm:ss");
            }

            return field.Value.ToString();
        });

        return string.Join(",", values);
    }

    private static string EscapeCsvField(string field)
    {
        if (string.IsNullOrEmpty(field)) return string.Empty;
        return field.Replace("\"", "\"\"");
    }
}