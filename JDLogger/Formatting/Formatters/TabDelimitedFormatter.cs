using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Models;

namespace JDLoggerV1.Formatting.Formatters;

/// <summary>
/// 탭으로 구분된 텍스트 형식의 로그 포맷터
/// </summary>
public class TabDelimitedFormatter(string header = null) : ILogFormatter<ILogModel>
{
    private static readonly Lazy<TabDelimitedFormatter> instance = new(() => new TabDelimitedFormatter());
    public static TabDelimitedFormatter Instance => instance.Value;


    private readonly string _header = header;

    public string GetHeader => _header;

    public string Format(LogFormatData data)
    {
        return string.Join("\t",
            data.Fields.Select(FormatValue)
        );
    }

    private string FormatValue(LogField field)
    {
        if (field.Value == null)
            return string.Empty;

        if (field.Type == typeof(DateTime))
            return ((DateTime)field.Value).ToString("yyyy-MM-dd HH:mm:ss");

        return field.Value.ToString();
    }
}
