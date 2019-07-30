# Controller

代码
``` C#
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<WeatherForecast> Get()
    {
        var rng = new Random();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
```

### ApiController

ModelBing 默认不支持form的ContentType，只支持route，queryString

在进入action之前有model validation，如验证不过返回http 400

### Route("[controller]")

路由模板，也就是endpoint

### 设计API原则

* 版本化
* API Model ≠ TableRow
* 请勿在额外包装一层状态码
    ``` json
    {
        "code":200,
        "data":{
            //...
        }
    }
    ```

    这样做的坏处

    1. 对消费方特别不友好
    2. 对重试特别糟糕
    3. 丑陋的code逻辑
    4. Gateway的健康检查不支持
    5. 网络级别的通用service mash无法支持 (k8s)



常规来讲一种资源应具有以下API
* post

    创建
* put

    修改
* delete

    删除
* patch

    部分修改
* get

    读取

相关链接

* https://blog.igevin.info/posts/restful-architecture-in-general/
* https://github.com/microsoft/api-guidelines
