using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;
using JDLoggerV1.Formatting;
using Microsoft.Extensions.Logging;
using System.Text;

namespace JDLoggerV1.Persistence.Extensions;

/// <summary>
/// 로거 관련 확장 메서드들을 제공합니다.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// 오늘 기록된 로그만 필터링합니다.
    /// </summary>
    public static IEnumerable<TModel> Today<TModel>(this IEnumerable<TModel> logs) where TModel : ILogModel
        => logs.Where(x => x.Time.Date == DateTime.Today);

    /// <summary>
    /// 특정 기간 동안의 로그만 필터링합니다.
    /// </summary>
    public static IEnumerable<TModel> Between<TModel>(this IEnumerable<TModel> logs, DateTime start, DateTime end) where TModel : ILogModel
        => logs.Where(x => x.Time >= start && x.Time <= end);

    /// <summary>
    /// 지정된 로그 레벨의 항목만 필터링합니다.
    /// </summary>
    public static IEnumerable<LogEntity> OfLevel(this IEnumerable<LogEntity> logs, LogLevel level)
        => logs.Where(x => x.Level == level);

    /// <summary>
    /// 최근 N개의 로그만 반환합니다.
    /// </summary>
    public static IEnumerable<TModel> Recent<TModel>(this IEnumerable<TModel> logs, int count) where TModel : ILogModel
        => logs.OrderByDescending(x => x.Time).Take(count);

    /// <summary>
    /// 로그를 파일로 내보냅니다.
    /// </summary>
    public static void Export<TModel>(this IEnumerable<TModel> logs, string path, ILogFormatter<TModel> formatter)
    where TModel : ILogModel
    {
        // 전체 경로를 정규화
        string fullPath = Path.GetFullPath(path);

        // 디렉토리 경로 추출
        string? directory = Path.GetDirectoryName(fullPath);

        // 디렉토리가 존재하지 않으면 생성
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var dataPreparer = new LogDataPreparer();
        var lines = new List<string>();

        // 헤더 추가
        var header = formatter.GetHeader;
        if (!string.IsNullOrEmpty(header))
        {
            lines.Add(header);
        }

        // 각 로그 항목을 포맷팅
        foreach (var log in logs)
        {
            var formatData = dataPreparer.PrepareData(log);
            string formattedLine = formatter.Format(formatData);
            lines.Add(formattedLine);
        }

        File.WriteAllLines(fullPath, lines, Encoding.UTF8);
    }

}