using JDLoggerV1.Core.Abstractions;
using JDLoggerV1.Core.Models;
using JDLoggerV1.Formatting.Attributes;
using SQLite;

namespace JDLoggerV1.Test.Models;

// 사용자 정의 로그 모델
[Table("App.ProductLog")]
public class ProductLog : ILogModel
{
    [ExportIgnore] public string Scope { get; set; }
    [ExportIgnore] public DateTime Time { get; set; }

    public string LotId { get; set; }
    public string ProductName { get; set; }
}
