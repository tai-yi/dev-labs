# 理解代码

## 入口以及启动

Program
``` C#
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
```

Startup
```C#

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
```

### 执行顺序

    Program.Main
        ↓
    Program.CreateHostBuilder
        ↓
    Startup 构造函数 
        ↓
    Startup.ConfigureServices(IServiceCollection services)
        ↓
    Configure(IApplicationBuilder app, IWebHostEnvironment env)

    到此Web服务器启动完成

### 深入Program

我们修改`Program.CreateHostBuilder`

``` C#
public static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((ctx, builder) =>
        {
            /*
                这个builder是什么?
                builder里有什么?
                ctx里有什么
            */
        })
        .ConfigureLogging((ctx, builder) =>
        {

        })
        .ConfigureServices((ctx, builder) =>
        {
            //这里能做什么呢?
        })
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
        //ConfigureAppConfiguration、ConfigureLogging、ConfigureServices谁先谁后
```

### 默认配置源

`ConfigureAppConfiguration((ctx, builder)`

1. `ChainedConfigurationSource`
    
    ConfigurationBuilder中使用另外的Ionfiguration
    
2. `JsonFileConfigurationSource`

    `appsettings.json`    
3. `JsonFileConfigurationSource`
    
    `appsettings.{env}.json` Development 环境下就是 `appsettings.Development.json`    
4. `EnvironmentVariablesConfigurationSource`

    环境变量

5. `CommandLineConfigurationSource`

    命令行参数

以上ConfigurationSources组成一个ConfigurationRoot

读取顺序从后向前 5 -> 1, 非null为止

Other docs
* http://www.sohu.com/a/321437201_505923

### 配置日志组件

`ConfigureLogging((ctx, builder)`

在此处添加其他的Log组件，Console、NLog等等

在AspNetcore中，日志组件的层次结构

    ILoggerFactory
        ILoggerProvider
            ILogger

### 配置DI 容器

`ConfigureServices((ctx, builder)`

### 配置web服务器

``` C#
ConfigureWebHostDefaults(webBuilder =>
{
    //导向到Startup 配置AspNetCore服务及中间件
    webBuilder.UseStartup<Startup>();
})
```

Starup [next->](session3.md)