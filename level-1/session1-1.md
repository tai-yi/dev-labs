# 高性能日志

## 使用 [LoggerMessage](https://github.com/aspnet/Extensions/blob/55518d79834d3319c91f40b449d028338b129ed6/src/Logging/Logging.Abstractions/src/LoggerMessage.cs#L100)

下面我们来做一个有趣的实验

1. 创建ConsoleApp

2. 编辑csprojfile,添加以下Nuget Package 

    ``` xml
    <!--请注意调整版本-->
    <ItemGroup>
        <PackageReference Include="BenchmarkDotNet" Version="0.11.5" />
        <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="2.2.0" />
        <PackageReference Include="Microsoft.Extensions.Logging.Console" Version="2.2.0" />
    </ItemGroup>
    ```

3. 修改 `Program.cs`
    ```
    public static void Main(string[] args) => BenchmarkRunner.Run<LogBenchMarkEntry>();
    ```
4. 添加文件 `MyService`
    ```C#
    public class MyService
    {
        private readonly ILogger _logger;

        public MyService(ILogger<MyService> logger)
        {
            _logger = logger;
        }

        public void RunWithNormalLog(MyServiceModel model)
        {
            _logger.LogInformation("Run model kind '{kind}' count '{count}'", model.Kind, model.Count);
        }

        public void RunWithOptLog(MyServiceModel model)
        {
            _logger.MyServiceRun(model);
        }
    }

    public class MyServiceModel
    {
        public string Kind { get; set; }

        public int Count { get; set; }
    }

    internal static class LogExtensions
    {
        private static readonly Action<ILogger, string, int, Exception> _myServiceRun;

        static LogExtensions()
        {
            _myServiceRun = LoggerMessage.Define<string, int>(
                LogLevel.Information,
                new EventId(0, nameof(MyServiceRun)),
                "Run model kind '{kind}' count '{count}'");
        }

        public static void MyServiceRun(this ILogger logger, MyServiceModel model)
        {
            _myServiceRun(logger, model.Kind, model.Count, null);
        }
    }
    ```

5. 添加`LogBenchMarkEntry`

    ```C#
    public class LogBenchMarkEntry
    {
        public LogBenchMarkEntry()
        {
            var services = new ServiceCollection()
            //如果console输出会影响benchmark runner
                .AddLogging()
                .AddTransient<MyService>()
                .BuildServiceProvider();
            _service = services.GetRequiredService<MyService>();
            _data = new MyServiceModel()
            {
                Count = 100,
                Kind = "kind1"
            };

        }

        private readonly MyServiceModel _data;
        private readonly MyService _service;
        
        [Benchmark]
        public void RunWithNormalLog()
        {
            _service.RunWithNormalLog(_data);
        }

        [Benchmark]
        public void RunWithOptLog()
        {
            _service.RunWithOptLog(_data);
        }
    }
    ```

6. 编译运行

    `dotnet run -c release`

    很快就看到很有意思的对比结果

Ref: https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/logging/loggermessage?view=aspnetcore-3.0
