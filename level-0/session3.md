# Starup.cs

## 构造函数

``` C#
public Startup(IConfiguration configuration)
{
    Configuration = configuration;
}
```

其实这里还可以支持另外一个接口的注入

```C#
public Startup(IConfiguration configuration, IHostEnvironment environment)
{
    Configuration = configuration;
}
```

## 配置MVC服务(DI容器配置)

```C#
public void ConfigureServices(IServiceCollection services)
{
    //webApi 相关服务
    services.AddControllers();
    //尽量使用扩展方法聚合模块，减少Startup的代码量
}
```

## 配置中间件

这个方法能够注入使用一切在DI容器配置的服务
``` C#
public void Configure(IApplicationBuilder app)
{        
    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

### 理解中间件

* https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-2.2

* https://www.cnblogs.com/stulzq/p/7760648.html

#### Endpoint中间件

    /api/*      //Rest API
    /hub/*      //SignalR
    /rpc/*      //GRPC

以上endpoints, 如果使用传统的MVC的中间件,会得到很糟糕的体验(特别深的pipeline,低性能).

剥离Endpoint不仅仅局限于MVC可以让扩展性更好，更好的Url控制

比如以上endpoints拥有各自的授权认证机制，在这种情况下就可以分别处理

## 使用IApplicationLifetime -> IHostApplicationLifetime

在3.0中 `IApplicationLifetime` 被标记为废弃的, 请使用  `IHostApplicationLifetime`

``` C#
public void Configure(IApplicationBuilder app, IHostApplicationLifetime lifetime)
{
    lifetime.ApplicationStarted.Register(() => {
        //耗时较长的代码，减少WebServer的启动时间
        //如果是BackgroundWorker，请使用Worker
    });

    app.UseRouting();

    app.UseAuthorization();

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });
}
```

高性能日志 [next](session4.md)