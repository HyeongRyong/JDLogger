using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Formatter;
using JDLoggerV1.Formatting.Formatters;

namespace JDLoggerV1.Formatting;


/// <summary>
/// 로그 포맷터 팩토리
/// </summary>
public static class JDLoggerFormatter
{
    /// <summary>
    /// CSV 형식의 로그 포맷터
    /// </summary>
    public static ILogFormatter<ILogModel> Csv => CsvLogFormatter.Instance;

    /// <summary>
    /// JSON 형식의 로그 포멧터
    /// </summary>
    public static ILogFormatter<ILogModel> Json => JsonLogFormatter.Instance;

    /// <summary>
    /// Tab(\t)로 구분 된 형식의 로그 포멧터
    /// </summary>
    public static ILogFormatter<ILogModel> Tabs => TabDelimitedFormatter.Instance;
}