## Razor

### 模板库工程

csproj文件
```XML
<Project Sdk="Microsoft.NET.Sdk.Razor">
  <PropertyGroup>
    <TargetFramework>netcoreapp3.0</TargetFramework> <!--默认是.netstandard , 按需修改-->
    <RazorCompileOnBuild>true</RazorCompileOnBuild>
    <!-- 可以自定义Razor输出程序集名称，默认: xxxx.Views.dll -->
    <!--<RazorTargetName>xxx.Cust.Views</RazorTargetName>-->
    <AddRazorSupportForMvc>true</AddRazorSupportForMvc>    
  </PropertyGroup>  
  <ItemGroup>
    <FrameworkReference Include="Microsoft.AspNetCore.App"/>    
  </ItemGroup>
</Project>
```
[SampleLib](sample)

调用代码如下
```C# 

var services = new ServicesCollection().AddTemplates().BuildServiceProvider();

var templateService = services.GetRequiredService<IRazorViewTextRenderer>();
var model = new .... //some you model object
var text = templateService.RenderViewToStringAsync("/Views/Notification.cshtml", model);

```

