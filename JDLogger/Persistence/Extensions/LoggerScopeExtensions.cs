using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;
using JDLoggerV1.Persistence.Scopes;
using Microsoft.Extensions.Logging;

namespace JDLoggerV1.Persistence.Extensions;

public static class LoggerScopeExtensions
{
    /// <summary>
    /// 특정 타입의 로그가 기록될 때 실행될 처리기를 등록
    /// </summary>
    public static JDLoggerScope<TScope> WhenLogged<TScope, TModel>(this JDLoggerScope<TScope> scope, Action<TModel> handler) 
        where TModel : ILogModel
    {
        scope.OnLogged = logObj => {
            if (logObj is TModel typedLog)
            {
                handler(typedLog);
            }
        };
        return scope;
    }

    /// <summary>
    /// 여러 타입의 로그에 대한 처리기들을 등록
    /// </summary>
    public static JDLoggerScope<TScope> WhenLogged<TScope>(
        this JDLoggerScope<TScope> scope, 
        Action<LogEntity> defaultHandler, 
        params (Type LogType, Action<ILogModel> Handler)[] typeHandlers)
    {
        scope.OnLogged = logObj => {
            if (logObj is LogEntity logEntity)
            {
                defaultHandler(logEntity);
                return;
            }

            foreach (var (logType, handler) in typeHandlers)
            {
                if (logType.IsInstanceOfType(logObj))
                {
                    handler((ILogModel)logObj);
                    break;
                }
            }
        };
        return scope;
    }

    /// <summary>
    /// 에러 로그가 기록될 때 실행될 처리기를 등록
    /// </summary>
    public static JDLoggerScope<TScope> WhenError<TScope>(this JDLoggerScope<TScope> scope, Action<LogEntity> handler)
    {
        var existingHandler = scope.OnLogged;
        scope.OnLogged = logObj => {
            if (logObj is LogEntity logEntity && logEntity.Level == LogLevel.Error)
            {
                handler(logEntity);
            }
            existingHandler?.Invoke(logObj);
        };
        return scope;
    }
}