using JDLoggerV1.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace JDLoggerV1.Persistence.Scopes;

public class JDLoggerAdapter(ILoggerScope scope) : ILogger
{
    private readonly ILoggerScope _scope = scope;

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        _scope.Log(logLevel, formatter(state, exception));
    }
}