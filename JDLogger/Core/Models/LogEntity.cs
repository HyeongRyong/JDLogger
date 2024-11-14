using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Attributes;
using Microsoft.Extensions.Logging;
using SQLite;

namespace JDLoggerV1.Core.Models;

/// <summary>
/// 기본 로그 엔티티 모델
/// </summary>
[Table("App.JdLog")]
public class LogEntity : ILogModel
{
    [PrimaryKey, AutoIncrement]
    [ExportIgnore]  // Export 출력에서 제외
    public int Id { get; set; }
    
    public DateTime Time { get; set; } = DateTime.Now;
    public LogLevel Level { get; set; } = LogLevel.None;
    public string Scope { get; set; }


    public string Message { get; set; }
    public string DumpData { get; set; }
    public string ExceptionType { get; set; }  // 예외 타입 (간단한 검색용)
    public string ExceptionData { get; set; }  // 전체 예외 정보 (JSON)
}