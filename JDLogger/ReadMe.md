# JDLogger

.NET 애플리케이션을 위한 경량화된 SQLite 기반의 로깅 라이브러리입니다.

프로그램 로깅과 데이터 로깅을 모두 지원하며, 구조화된 로그 데이터를 쉽게 저장하고 조회할 수 있습니다.

## 주요 기능

- SQLite 기반의 영구 저장소
- 계층적 로깅 스코프 지원
- 구조화된 로그 데이터 저장
- 예외 정보의 상세한 기록
- 다양한 형식(CSV 등)으로 로그 내보내기
- 커스텀 로그 모델 지원
- 손쉬운 예외 처리 및 로깅
- Thread-Safe하게 설계된 로깅

## 시작하기

### 설치

```shell
dotnet add package JDLogger
```

### 초기화
- **초기화를 하지 않고 사용하는 경우 `jd__program_log.db` 라는 파일로 자동으로 초기화됩니다.**
```csharp
// 기본 초기화
JDLogger.Initialize("my_log.db");

// 커스텀 매퍼와 함께 초기화
JDLogger.Initialize("my_log.db", new[] { 
    new ProductLogMapper()
});
```

## 사용 예제

### 1. 기본적인 로깅

```csharp
static void ApplicationLogExample()
{
    var log = JDLogger.BeginScope<Program>();
    log.Log(LogLevel.Information, "프로그램 시작");
    
    // 로그 조회 및 내보내기
    log.Where(entity => entity.Level == LogLevel.Information)
       .Export("log.csv", JDLoggerFormatter.Csv);
}
```

### 2. 커스텀 데이터 로깅

```csharp
static void ProductLogExample()
{
    var log = JDLogger.BeginScope<TestSequence>();
    
    var productLog = new ProductLog
    {
        LotId = "LOT001",
        ProductName = "TestProduct",
    };
    log.Log(productLog);
}
```

### 3. 로그 처리기
- 로그가 작성 된 이후 처리를 위한 처리기 등록이 가능합니다.
```csharp
static void ProductLogExample()
{
    var log = JDLogger.BeginScope<TestSequence>();
    
    // 로그 처리기 등록
    log.WhenLogged<TestSequence, ProductLog>(x => {
        Console.WriteLine(x.ProductName); 
    });
    
    // 테스트용 로그 작성
    var productLog = new ProductLog
    {
        LotId = "LOT001",
        ProductName = "TestProduct",
    };

    log.Log(productLog);
}
```

### 4. 예외 처리 및 로깅
- 예외가 발생할 가능성이 높은 작업을 기록합니다.
- `operationName`를 통해 어떤 작업 도중 예외가 발생 했는지 작업 단위의 추적이 가능합니다.
```csharp
static void UseTryCatchExample()
{
    var log = JDLogger.BeginScope<TestSequence>();
    
    // 자동 예외 로깅
    log.TryCatch(() =>
    {
        string[] s = ["a", "b", "c"];
        for (int i = 0; i < 100; i++)
        {
            Console.WriteLine(s[i]); // will throw exception
        }
    }, operationName: "indexReferenceJob", @continue: true);

    // 오늘 발생한 에러 로그 조회
    foreach (var ex in log.All().Today().Where(entity => entity.Level == LogLevel.Error))
    {
        Console.WriteLine(ex.ExceptionData);
    }
}
```

### 5. 타임아웃 로깅

```csharp
static void TimeoutExample()
{
    var logger = JDLogger.BeginScope();

    // 동기 버전 사용
    bool isTimeout = logger.Timeout(() =>
    {
        Console.WriteLine("시작");
        Thread.Sleep(5000); // 오래 걸리는 작업
        Console.WriteLine("중지");
    }, milliseconds: 3000, operationName: "긴 작업");

    if (isTimeout)
    {
        Console.WriteLine("타임아웃 발생");
    }
}
```


### 6. 객체 상태 덤프

```csharp
static void UseDumpExample()
{
    var log = JDLogger.BeginScope<DumpExample>();
    
    // 객체 상태 덤프
    var settings = new AppSettings() { Identifier = "Test" };
    log.Dump(settings, LogLevel.Critical, message: "객체 상태 덤프");
    
    // 예외 덤프
    try
    {
        throw new NullReferenceException("Exception 데이터 덤프 예제");
    }
    catch (Exception ex)
    {
        log.Dump(ex);
    }
}
```

## 로그 조회를 위한 확장 메서드

### 시간 기반 필터링

```csharp
// 오늘 기록된 로그만 조회
var todayLogs = log.All().Today();

// 특정 기간의 로그 조회
var lastWeekLogs = log.All().Between(
    DateTime.Now.AddDays(-7), 
    DateTime.Now
);
```

### 로그 레벨 필터링

```csharp
// 에러 로그만 조회
var errorLogs = log.All().OfLevel(LogLevel.Error);

// 크리티컬 로그만 조회 후 CSV로 내보내기
log.All()
   .OfLevel(LogLevel.Critical)
   .Export("critical-logs.csv", JDLoggerFormatter.Csv);
```

### 최근 로그 조회

```csharp
// 최근 100개의 로그 조회
var recentLogs = log.All().Recent(100);

// 최근 10개의 에러 로그 조회
var recentErrors = log.All()
    .OfLevel(LogLevel.Error)
    .Recent(10);
```

### 필터 조합하기

```csharp
// 오늘 발생한 에러 중 최근 5개만 조회
var recentTodayErrors = log.All()
    .Today()
    .OfLevel(LogLevel.Error)
    .Recent(5);

// 지난 주의 크리티컬 로그를 CSV로 내보내기
log.All()
   .Between(DateTime.Now.AddDays(-7), DateTime.Now)
   .OfLevel(LogLevel.Critical)
   .Export("last-week-critical.csv", JDLoggerFormatter.Csv);
```

## 커스텀 로그 모델

사용자 정의 로그 모델을 생성하여 특정 도메인의 데이터를 로깅할 수 있습니다:

```csharp
public class ProductLog : ILogModel
{
    [ExportIgnore] public string Scope { get; set; }
    public DateTime Time { get; set; }
    public string LotId { get; set; }
    public string ProductName { get; set; }
}
```

## 로그 내보내기

로그를 다양한 형식으로 내보낼 수 있습니다:

```csharp
// CSV로 내보내기
log.All().Export("logs.csv", JDLoggerFormatter.Csv);

// 헤더를 지정해서 CSV로 내보내기
log.All().Export("logs.csv", new CsvLogFormatter("a,b,c,d,e,f"));

// Json로 내보내기
log.All().Export("logs.csv", JDLoggerFormatter.Json);

// Tab(\t)로 구분된 포멧터로 내보내기
log.All().Export("logs.txt", JDLoggerFormatter.Tabs);

// 헤더를 지정해서서 Tab(\t)로 구분된 포멧터로 내보내기
log.All().Export("logs.txt", new TabDelimitedFormatter("a\tb\tc\td\te\tf"));

// 커스텀 포맷터 사용
var customFormatter = new CustomLogFormatter();
log.All().Export("logs.txt", customFormatter);
```

- `ExportIgnore` Attribute를 사용하여 내보내는 컬럼을 무시할 수 있습니다.
```csharp

// 커스텀 데이터 로깅 모델의 Scope 출력 제어어
public class ProductLog : ILogModel
{
    [ExportIgnore] public string Scope { get; set; }
    public DateTime Time { get; set; }
    public string LotId { get; set; }
    public string ProductName { get; set; }
}

```
