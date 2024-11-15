﻿using Microsoft.Extensions.Logging;
using SQLite;
using System.Text.Json.Serialization;
using System.Text.Json;
using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;

namespace JDLoggerV1.Persistence.Scopes;

/// <summary>
/// 특정 클래스에 대한 로깅 스코프를 제공하는 클래스
/// </summary>
/// <typeparam name="T">로그가 속할 클래스 타입</typeparam>
public class JDLoggerScope<T> : ILoggerScope
{
    private readonly SQLiteConnection _db;
    private readonly string _scopeName;
    private readonly object _logLock = new();
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        Converters = { new JsonStringEnumConverter() }
    };

    private static readonly JsonSerializerOptions _indented = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    internal JDLoggerScope(SQLiteConnection db)
    {
        _db = db;
        _scopeName = typeof(T).Name;
    }

    /// <summary>
    /// 로그가 기록된 후 실행될 콜백입니다.
    /// </summary>
    public Action<object> OnLogged { get; set; }

    /// <summary>
    /// 지정된 모델을 사용하여 로그를 기록합니다.
    /// </summary>
    public void Log<TModel>(TModel model) where TModel : ILogModel, new()
    {
        model.Scope = _scopeName;
        model.Time = DateTime.UtcNow;

        var mapper = JDLogger.GetMapper<TModel>();

        lock (_logLock)
        {
            mapper.Insert(_db, model);
        }

        OnLogged?.Invoke(model);
    }

    /// <summary>
    /// LogEntity를 사용하여 로그를 기록합니다.
    /// </summary>
    public void Log(LogLevel level, string message)
    {
        var logEntity = new LogEntity
        {
            Scope = _scopeName,
            Level = level,
            Time = DateTime.UtcNow,
            Message = message
        };

        Log(logEntity);
    }

    /// <summary>
    /// try-catch 블록에서 사용할 수 있는 편의 메서드입니다.
    /// </summary>
    public void TryCatch(Action action, string operationName = null, bool @continue = true)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            Dump(ex, LogLevel.Error, $"작업(`{operationName ?? "operation"}`)중 오류가 발생 했습니다.");
            if (@continue is false)
                throw;
        }
    }

    /// <summary>
    /// try-catch 블록에서 사용할 수 있는 편의 메서드입니다. (비동기 버전)
    /// </summary>
    public async Task TryCatchAsync(Func<Task> action, string operationName = null, bool @continue = true)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            Dump(ex, LogLevel.Error, $"비동기 작업(`{operationName ?? "operation"}`)중 오류가 발생 했습니다.");
            if (@continue is false)
                throw;
        }
    }

    /// <summary>
    /// 지정된 객체를 로그에 기록 합니다.
    /// </summary>
    public void Dump<TValue>(TValue value, LogLevel level = LogLevel.Information, string message = null, object additionalData = null)
    {
        var logEntity = new LogEntity
        {
            Scope = _scopeName,
            Time = DateTime.UtcNow,
            Level = level
        };

        if (value is Exception ex)
        {
            var exInfo = ExceptionInfo.FromException(ex);
            logEntity.Message = message ?? ex.Message;
            logEntity.ExceptionType = ex.GetType().FullName;
            logEntity.ExceptionData = JsonSerializer.Serialize(exInfo, _jsonOptions);

            if (additionalData != null)
            {
                logEntity.DumpData = JsonSerializer.Serialize(additionalData, _jsonOptions);
            }
        }
        else
        {
            logEntity.Message = message ?? $"Dump of {typeof(TValue).Name}";
            logEntity.DumpData = JsonSerializer.Serialize(value, _jsonOptions);

            if (additionalData != null)
            {
                throw new ArgumentException("additionalData는 Exception을 덤프할 때만 사용할 수 있습니다.");
            }
        }

        Log(logEntity);
    }

    /// <summary>
    /// LogEntity의 모든 로그 항목을 반환합니다.
    /// </summary>
    public IEnumerable<LogEntity> All()
    {
        lock (_logLock)
        {
            return _db.Table<LogEntity>()
                     .Where(x => x.Scope == _scopeName)
                     .ToList();
        }
    }

    /// <summary>
    /// 지정된 모델의 모든 로그 항목을 반환합니다.
    /// </summary>
    public IEnumerable<TModel> All<TModel>() where TModel : ILogModel, new()
    {
        lock (_logLock)
        {
            return _db.Table<TModel>()
                     .Where(x => x.Scope == _scopeName)
                     .ToList();
        }
    }

    /// <summary>
    /// LogEntity의 로그 항목 중 조건을 만족하는 항목을 반환합니다.
    /// </summary>
    public IEnumerable<LogEntity> Where(Func<LogEntity, bool> predicate)
        => Where<LogEntity>(predicate);

    /// <summary>
    /// 조건에 맞는 로그 항목들을 검색합니다.
    /// </summary>
    public IEnumerable<TModel> Where<TModel>(Func<TModel, bool> predicate) where TModel : ILogModel, new()
    {
        return All<TModel>()
                 .Where(predicate)
                 .ToList();
    }

    /// <summary>
    /// 특정 예외 타입의 로그를 검색합니다.
    /// </summary>
    public IEnumerable<LogEntity> GetExceptionLogs<TException>() where TException : Exception
    {
        var exceptionType = typeof(TException).FullName;
        lock (_logLock)
        {
            return _db.Table<LogEntity>()
                     .Where(x => x.Scope == _scopeName && x.ExceptionType == exceptionType)
                     .OrderByDescending(x => x.Time)
                     .ToList();
        }
    }
}