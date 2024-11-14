using JDLoggerV1.Formatting.Models;

namespace JDLoggerV1.Core.Abstractions;

/// <summary>
/// 형식화된 로그 출력을 정의하는 인터페이스
/// </summary>
public interface ILogFormatter<TModel> where TModel : ILogModel
{
    string Format(LogFormatData data);
    string GetHeader { get; }
}