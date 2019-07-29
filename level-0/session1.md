# 开始AspNetCore

## 环境安装 (3.0)

* 下载安装SDK/VisualStudio(2017,2019)

    SDK : https://github.com/dotnet/core/release

    VisualStudio: https://visualstudio.com

## 创建工程

* 命令行
  
    1. 创建文件夹 WebApp1
    2. cd `WebApp1`
    3. `dotnet new web`

* VisualStudio GUI
  
    新建工程->ConsoleApp


## 理解工程结构

    📦WebApp1
    ┣ 📂Controllers
    ┃ ┗ 📜WeatherForecastController.cs    
    ┣ 📂Properties
    ┃ ┗ 📜launchSettings.json
    ┣ 📜appsettings.Development.json
    ┣ 📜appsettings.json
    ┣ 📜Program.cs
    ┣ 📜Startup.cs
    ┣ 📜WeatherForecast.cs
    ┗ 📜WebApp1.csproj

* WebApp1.csproj

    CSharp Project 文件,编译必要文件,描述了SDK、平台、包引用、编译逻辑、编译资源、内嵌资源、输出拷贝资源

* Program.cs
  
    程序入口文件 (`Main`函数)

* Controllers/WeatherForecastController
  
    MVC的 控制器

* Properties/launchSettings.json

    调试启动配置信息

* appsettings.*.json

    应用程序配置文件

## 启动程序

* 命令行
  
   在程序目录(csproj文件同级) `dotnet run`

* VisualStudio

    按下 F5

理解代码 [next->](session2.md)