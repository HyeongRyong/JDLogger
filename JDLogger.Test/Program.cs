using JDLogger.Persistence.Extensions;
using JDLoggerV1.Formatting;
using JDLoggerV1.Persistence.Extensions;
using JDLoggerV1.Test.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace JDLoggerV1.Test;

internal class Program
{
    static void Main(string[] args)
    {

        // 일반적인 프로그램 로깅 및 조회, 내보내기 예제
        //ApplicationLogExample();

        // 제품 데이터 로깅용 예제
        //ProductLogExample();

        // 예외 발생 가능성의 민감한 작업(try-catch)의 로깅 예제
        /*UseTryCatchExample();

        // Dump 메소드 사용 예제
        UseDumpExample();
        */

        // 타임아웃 예제
        TimeoutExample();

        Console.ReadLine();
    }


    /// <summary>
    /// 일반적인 프로그램 로깅 및 조회, 내보내기 예제
    /// </summary>
    static void ApplicationLogExample()
    {
        var log = JDLogger.BeginScope<Program>();

        log.Log(LogLevel.Information, "프로그램 시작");
    }

    /// <summary>
    /// 제품 데이터 로깅용 예제
    /// </summary>
    static void ProductLogExample()
    {
        var log = JDLogger.BeginScope<TestSequence>();
        
        // 테스트용 로그 작성
        var productLog = new ProductLog
        {
            LotId = "LOT001",
            ProductName = "TestProduct",
        };

        log.WhenLogged<TestSequence, ProductLog>(x => {
            Console.WriteLine(x.ProductName); 
        });
        log.Log(productLog);

        log.All<ProductLog>().Export("test/log.txt", JDLoggerFormatter.Tabs);
    }

    static void UseTryCatchExample()
    {
        var log = JDLogger.BeginScope<TestSequence>();

        // 지정된 작업을 실행하고 발생하는 예외를 자동으로 로깅합니다.
        // continue가 true인 경우 예외가 발생해도 예외를 throw 하지 않고 계속 진행합니다.
        log.TryCatch(() =>
        {
            // 예외가 발생할 가능성이 높은 작업
            string[] s = ["a", "b", "c"];
            for (int i = 0; i < 100; i++)
            {
                // should be throw exception
                Console.WriteLine(s[i]);
            }
        }, operationName: "indexReferenceJob", @continue: true);

        foreach (var ex in log.All().Today().Where(entity => entity.Level == LogLevel.Error))
        {
            Console.WriteLine(ex.ExceptionData);
        }
    }

    private static void UseDumpExample()
    {
        var log = JDLogger.BeginScope<DumpExample>();

        var c = new AppSettings() { Identifier = "Test" };
        log.Dump(c, LogLevel.Critical, message: "객체 상태 덤프"); // 클래스 상태 덤프 예제

        // 예외 발생 덤프 예제
        try
        {
            throw new NullReferenceException("Exception 데이터 덤프 예제");
        }
        catch (Exception ex)
        {
            log.Dump(ex);
        }
        
        foreach (var l in log.Where(entity => entity.Scope == nameof(DumpExample)))
        {
            Console.WriteLine(l.Message);
        }
    }

    private static void TimeoutExample()
    {
        var logger = JDLogger.BeginScope();

        // 동기 버전 사용
        bool isTimeout = logger.Timeout(() =>
        {
            Console.WriteLine("시작");
            Thread.Sleep(5000); // 오래 걸리는 작업
            Console.WriteLine("중지");
        }, milliseconds: 3000, operationName: "긴 작업", @continue: true);

        if (isTimeout)
        {
            Console.WriteLine("타임아웃 발생");
        }
    }

    #region etc
    private class AppSettings
    {
        public string Identifier { get; set; }
        public ProgramState State { get; set; } = ProgramState.Test;

    }

    public enum ProgramState
    {
        None,
        Test,
    }

    private class TestSequence { }
    private class DumpExample { }
    #endregion
}
