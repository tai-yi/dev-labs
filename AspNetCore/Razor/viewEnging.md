## 试图引擎

### 调用顺序

#### `ControllerAction` 到`Http Response`

```
Controller Action
       ↓
   ViewResult
       ↓
IActionResultExecutor<ViewResult>     
//MVC 默认实现类: ViewResultExecutor: ViewExecutor,IActionResultExecutor<ViewResult>
    (1)↓
   FindView
       ↓
分别从//ICompositeViewEngine.GetView的
//success:true       (false)↘
        |      ICompositeViewEngine.FindView
        |         //success:true       (false)   ⇢ throw exception
        ↓               ↙
ViewExecutor.ExecuteAsync //写入到Http Response
```

#### 试图引擎查找IView原理

`ICompositeViewEngine` 里可以允许有多个 `IViewEngine` //运行时编译 与 Publish编译

**IRazorViewEngine**

**接口**

`IRazorViewEngine: IViewEngine`

**实现类**

`RazorViewEngine:IRazorViewEngine`

```
RazorViewEngine
//被缓存 no  yes  -> return
         ↓    
IRazorPageFactoryProvider //DefaultRazorPageFactoryProvider
         ↓
IViewCompilerProvider //DefaultViewCompilerProvider or RuntimeViewCompilerProvider
|
┣  DefaultViewCompilerProvider //ApplicationPartManager.PopulateFeature<ViewsFeature>
|       ↓ 
|      DefaultViewCompiler
|
┗  RuntimeViewCompilerProvider   //CSharpCompiler,RazorProjectEngine, RuntimeCompilationFileProvider
         ↓ 
       RuntimeViewCompiler //ApplicationPartManager.PopulateFeature<ViewsFeature>,RazorProjectEngine,CSharpCompiler

```
### 接口

* IViewEnging
    ```C#
    //
    // Summary:
    //     Defines the contract for a view engine.
    public interface IViewEngine
    {
        //
        // Summary:
        //     Finds the view with the given viewName using view locations and information from
        //     the context.
        //
        // Parameters:
        //   context:
        //     The Microsoft.AspNetCore.Mvc.ActionContext.
        //
        //   viewName:
        //     The name or path of the view that is rendered to the response.
        //
        //   isMainPage:
        //     Determines if the page being found is the main page for an action.
        //
        // Returns:
        //     The Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult of locating the view.
        //
        // Remarks:
        //     Use Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine.GetView(System.String,System.String,System.Boolean)
        //     when the absolute or relative path of the view is known.
        ViewEngineResult FindView(ActionContext context, string viewName, bool isMainPage);
        //
        // Summary:
        //     Gets the view with the given viewPath, relative to executingFilePath unless viewPath
        //     is already absolute.
        //
        // Parameters:
        //   executingFilePath:
        //     The absolute path to the currently-executing view, if any.
        //
        //   viewPath:
        //     The path to the view.
        //
        //   isMainPage:
        //     Determines if the page being found is the main page for an action.
        //
        // Returns:
        //     The Microsoft.AspNetCore.Mvc.ViewEngines.ViewEngineResult of locating the view.
        ViewEngineResult GetView(string executingFilePath, string viewPath, bool isMainPage);
    }
    ```
* ICompositeViewEngine

    ```C#
    public interface ICompositeViewEngine : IViewEngine
    {
        //
        // Summary:
        //     Gets the list of Microsoft.AspNetCore.Mvc.ViewEngines.IViewEngine this instance
        //     of Microsoft.AspNetCore.Mvc.ViewEngines.ICompositeViewEngine delegates to.
        IReadOnlyList<IViewEngine> ViewEngines { get; }
    }
    ```

