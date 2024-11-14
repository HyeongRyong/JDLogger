using JDLoggerV1.Core.Models;
using Microsoft.Extensions.Logging;

namespace JDLoggerV1.Core.Abstractions;

/// <summary>
/// 로깅 스코프에 대한 인터페이스를 정의합니다.
/// </summary>
public interface ILoggerScope
{
    /// <summary>
    /// 로그가 기록된 후 실행될 콜백입니다.
    /// </summary>
    Action<object> OnLogged { get; set; }

    /// <summary>
    /// 사용자 정의 로그 모델을 사용하여 로그를 기록합니다.
    /// </summary>
    /// <typeparam name="TModel">로그 모델 타입</typeparam>
    /// <param name="model">기록할 로그 모델 인스턴스</param>
    void Log<TModel>(TModel model) where TModel : ILogModel, new();

    /// <summary>
    /// 지정된 로그 레벨과 메시지로 로그를 기록합니다.
    /// </summary>
    /// <param name="level">로그 레벨</param>
    /// <param name="message">로그 메시지</param>
    void Log(LogLevel level, string message);

    /// <summary>
    /// 객체를 로그에 기록합니다.
    /// Exception인 경우 예외 정보를 상세히 기록하고, <br />
    /// 그 외의 객체는 JSON으로 직렬화하여 기록합니다. <br />
    /// </summary>
    /// <typeparam name="TValue">덤프할 객체의 타입</typeparam>
    /// <param name="value">덤프할 객체</param>
    /// <param name="level">로그 레벨</param>
    /// <param name="message">추가 메시지</param>
    /// <param name="additionalData">추가 데이터 (Exception인 경우에만 사용)</param>
    void Dump<TValue>(TValue value, LogLevel level = LogLevel.Information, string message = null, object additionalData = null);

    /// <summary>
    /// 지정된 모델 타입의 모든 로그 항목을 반환합니다.
    /// </summary>
    /// <typeparam name="TModel">로그 모델 타입</typeparam>
    /// <returns>로그 항목 컬렉션</returns>
    IEnumerable<TModel> All<TModel>() where TModel : ILogModel, new();

    /// <summary>
    /// 기본 로그 엔티티의 모든 항목을 반환합니다.
    /// </summary>
    /// <returns>로그 엔티티 컬렉션</returns>
    IEnumerable<LogEntity> All();

    /// <summary>
    /// 지정된 조건을 만족하는 로그 항목들을 반환합니다.
    /// </summary>
    /// <typeparam name="TModel">로그 모델 타입</typeparam>
    /// <param name="predicate">필터링 조건</param>
    /// <returns>조건을 만족하는 로그 항목 컬렉션</returns>
    IEnumerable<TModel> Where<TModel>(Func<TModel, bool> predicate) where TModel : ILogModel, new();

    /// <summary>
    /// 기본 로그 엔티티 중 지정된 조건을 만족하는 항목들을 반환합니다.
    /// </summary>
    /// <param name="predicate">필터링 조건</param>
    /// <returns>조건을 만족하는 로그 엔티티 컬렉션</returns>
    IEnumerable<LogEntity> Where(Func<LogEntity, bool> predicate);

    /// <summary>
    /// 지정된 작업을 실행하고 발생하는 예외를 자동으로 로깅합니다.
    /// </summary>
    /// <param name="action">실행할 작업</param>
    /// <param name="operationName">작업 이름 (선택사항)</param>
    /// <param name="continue">예외가 발생하면 작업을 중지시키지 않고 계속 진행 여부 (기본값: true)</param>
    void TryCatch(Action action, string operationName = null, bool @continue = true);

    /// <summary>
    /// 지정된 비동기 작업을 실행하고 발생하는 예외를 자동으로 로깅합니다.
    /// </summary>
    /// <param name="action">실행할 비동기 작업</param>
    /// <param name="operationName">작업 이름 (선택사항)</param>
    /// <param name="continue">예외가 발생하면 작업을 중지시키지 않고 계속 진행 여부 (기본값: true)</param>
    /// <returns>비동기 작업 완료를 나타내는 Task</returns>
    Task TryCatchAsync(Func<Task> action, string operationName = null, bool @continue = true);
}