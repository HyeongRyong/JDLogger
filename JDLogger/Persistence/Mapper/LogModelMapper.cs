using JDLoggerV1.Core.Abstractions;
using SQLite;

namespace JDLoggerV1.Persistence.Mapper;

/// <summary>
/// 기본 매퍼 구현
/// </summary>
internal class LogModelMapper<TModel> : ILogModelMapper<TModel> where TModel : ILogModel, new()
{
    void ILogModelMapper.Insert(SQLiteConnection db, ILogModel model)
    {
        if (model is TModel typedModel)
        {
            Insert(db, typedModel);
        }
    }

    public void Insert(SQLiteConnection db, TModel model)
    {
        db.Insert(model);
    }

    public Type GetModelType() => typeof(TModel);
}