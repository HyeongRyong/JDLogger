using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;
using JDLoggerV1.Persistence.Mapper;
using JDLoggerV1.Persistence.Scopes;
using Microsoft.Extensions.Logging;
using SQLite;
using System.Collections.Concurrent;

namespace JDLoggerV1;

/// <summary>
/// SQLite 기반의 로깅을 제공하는 로거 클래스
/// </summary>
public class JDLogger
{
    private static volatile SQLiteConnection _db;
    private static readonly ConcurrentDictionary<Type, ILogModelMapper> _mappers = new();
    private const string DEFAULT_DB_PATH = "jd__program_log.db";
    private static readonly object _initLock = new();

    /// <summary>
    /// 로거에서 사용할 데이터베이스와 매퍼들을 초기화합니다.
    /// </summary>
    /// <param name="dbPath">데이터베이스 파일 경로. .db 확장자는 생략 가능합니다.</param>
    /// <param name="mappers">사용할 로그 모델 매퍼들</param>
    public static void Initialize(string dbPath, IEnumerable<ILogModelMapper> mappers = null)
    {
        if (_db != null) return; // Double-check locking pattern

        lock (_initLock)
        {
            if (_db != null) return;

            string fullPath = dbPath.EndsWith(".db", StringComparison.OrdinalIgnoreCase) ? dbPath : $"{dbPath}.db";

            _db = new SQLiteConnection(Path.GetFullPath(fullPath));

            // 기본 매퍼 등록 및 테이블 생성
            RegisterMapper(new LogEntityMapper());
            _db.CreateTable<LogEntity>();

            // 추가 매퍼 등록 및 테이블 생성
            if (mappers != null)
            {
                foreach (var mapper in mappers)
                {
                    RegisterMapper(mapper);
                    _db.CreateTable(mapper.GetModelType());
                }
            }
        }
    }

    /// <summary>
    /// 새로운 매퍼를 등록합니다.
    /// </summary>
    public static void RegisterMapper(ILogModelMapper mapper)
    {
        var modelType = mapper.GetModelType();
        if (!_mappers.ContainsKey(modelType))
        {
            _mappers[modelType] = mapper;
        }
    }

    /// <summary>
    /// 제네릭 타입용 편의 메서드
    /// </summary>
    public static void RegisterMapper<TModel>(ILogModelMapper<TModel> mapper) where TModel : ILogModel
        => RegisterMapper((ILogModelMapper)mapper);

    internal static ILogModelMapper<TModel> GetMapper<TModel>() where TModel : ILogModel, new()
    {
        var modelType = typeof(TModel);
        return (ILogModelMapper<TModel>)_mappers.GetOrAdd(modelType, type =>
        {
            var mapper = new LogModelMapper<TModel>();

            if (_db != null)
            {
                lock (_initLock)
                {
                    _db.CreateTable<TModel>();
                }
            }

            return mapper;
        });
    }

    /// <summary>
    /// 새로운 로깅 스코프를 시작합니다.
    /// </summary>
    /// <typeparam name="TClass">로그가 속할 클래스 타입</typeparam>
    /// <returns>로깅 스코프 인스턴스</returns>
    public static JDLoggerScope<TClass> BeginScope<TClass>()
    {
        if (_db == null)
        {
            Initialize(DEFAULT_DB_PATH);
        }
        return new JDLoggerScope<TClass>(_db);
    }

    /// <summary>
    /// 새로운 로깅 스코프를 시작합니다.
    /// </summary>
    /// <returns>로깅 스코프 인스턴스</returns>
    public static JDLoggerScope<JDScope> BeginScope()
    {
        return BeginScope<JDScope>();
    }


    /// <summary>
    /// 의존성 주입을 위한 ILogger 인스턴스를 생성합니다.
    /// </summary>
    /// <typeparam name="TClass">로그가 속할 클래스 타입</typeparam>
    /// <returns>ILogger 인스턴스</returns>
    public static ILogger CreateLogger<TClass>()
    {
        if (_db == null)
        {
            Initialize(DEFAULT_DB_PATH);
        }
        var scope = new JDLoggerScope<TClass>(_db);
        return new JDLoggerAdapter(scope);
    }
}

public record JDScope();