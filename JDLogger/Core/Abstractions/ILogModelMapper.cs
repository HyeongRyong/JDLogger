using SQLite;

namespace JDLoggerV1.Core.Abstractions;

/// <summary>
/// 기본 매퍼 인터페이스
/// </summary>
public interface ILogModelMapper
{
    void Insert(SQLiteConnection db, ILogModel model);
    Type GetModelType();
}

/// <summary>
/// 타입별 매퍼 인터페이스
/// </summary>
public interface ILogModelMapper<TModel> : ILogModelMapper where TModel : ILogModel
{
    void Insert(SQLiteConnection db, TModel model);  // 구체적인 타입의 Insert 제공
}