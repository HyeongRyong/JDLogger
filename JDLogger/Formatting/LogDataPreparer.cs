using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Formatting.Attributes;
using JDLoggerV1.Formatting.Models;
using System.Collections.Concurrent;
using System.Reflection;

namespace JDLoggerV1.Formatting;

public class LogDataPreparer
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> PropertyCache = new();

    public LogFormatData PrepareData(ILogModel entity)
    {
        var properties = GetModelProperties(entity.GetType());
        var fields = properties.Select(prop => new LogField(
            prop.Name,
            prop.GetValue(entity),
            prop.PropertyType
        )).ToList();

        return new LogFormatData(fields, entity.GetType());
    }

    private PropertyInfo[] GetModelProperties(Type type)
    {
        return PropertyCache.GetOrAdd(type, t =>
        {
            return t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && !p.IsDefined(typeof(ExportIgnoreAttribute)))
                .OrderBy(p => !IsBaseProperty(p))
                .ToArray();
        });
    }

    private static bool IsBaseProperty(PropertyInfo prop)
    {
        return typeof(ILogModel).GetProperties().Any(p => p.Name == prop.Name);
    }
}