
namespace JDLoggerV1.Formatting.Models;

/// <summary>
/// 포맷팅을 위해 준비된 로그 데이터
/// </summary>
public class LogFormatData(IReadOnlyList<LogField> fields, Type modelType)
{
    public IReadOnlyList<LogField> Fields { get; } = fields;
    public Type ModelType { get; } = modelType;
}
