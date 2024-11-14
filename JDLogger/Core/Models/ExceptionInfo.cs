using System.Collections;

namespace JDLoggerV1.Core.Models;

public class ExceptionInfo
{
    public string Type { get; set; }
    public string Message { get; set; }
    public string StackTrace { get; set; }
    public string Source { get; set; }
    public Dictionary<string, string> Data { get; set; }
    public ExceptionInfo InnerException { get; set; }

    public static ExceptionInfo FromException(Exception ex)
    {
        if (ex == null) return null;

        var data = new Dictionary<string, string>();
        foreach (DictionaryEntry entry in ex.Data)
        {
            data[entry.Key?.ToString() ?? "null"] = entry.Value?.ToString() ?? "null";
        }

        return new ExceptionInfo
        {
            Type = ex.GetType().FullName,
            Message = ex.Message,
            StackTrace = ex.StackTrace,
            Source = ex.Source,
            Data = data,
            InnerException = FromException(ex.InnerException)
        };
    }
}