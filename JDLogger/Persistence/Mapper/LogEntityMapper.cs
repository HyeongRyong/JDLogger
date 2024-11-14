using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;
using SQLite;

namespace JDLoggerV1.Persistence.Mapper;

/// <summary>
/// 기본 로그 매퍼
/// </summary>
public class LogEntityMapper : ILogModelMapper<LogEntity>
{
    void ILogModelMapper.Insert(SQLiteConnection db, ILogModel model)
    {
        if (model is LogEntity logEntity)
        {
            Insert(db, logEntity);
        }
    }

    public void Insert(SQLiteConnection db, LogEntity model)
    {
        db.Insert(model);
    }

    public Type GetModelType() => typeof(LogEntity);
}
