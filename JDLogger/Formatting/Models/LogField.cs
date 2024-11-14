namespace JDLoggerV1.Formatting.Models;

/// <summary>
/// 로그 데이터의 단일 필드
/// </summary>
public class LogField(string name, object value, Type type)
{
    public string Name { get; } = name;
    public object Value { get; } = value;
    public Type Type { get; } = type;
}
