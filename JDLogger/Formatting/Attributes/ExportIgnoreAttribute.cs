using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JDLoggerV1.Formatting.Attributes;

/// <summary>
/// Export시 무시할 속성을 지정하는 특성
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ExportIgnoreAttribute : Attribute
{
}