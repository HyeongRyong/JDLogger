using Microsoft.Extensions.Logging;

namespace JDLoggerV1.Core.Abstractions;

/// <summary>
/// 로그 데이터의 기본 속성을 정의하는 인터페이스
/// </summary>
public interface ILogModel
{
    DateTime Time { get; set; }
    string Scope { get; set; }
}