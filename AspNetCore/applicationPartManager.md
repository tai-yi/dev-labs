
## ApplicationPartManager

在IServicesCollection 添加MvcCore的内置行为

Assembly `Microsoft.AspNetCore.Mvc.Core` class `MvcCoreServiceCollectionExtensions` [源代码链接](https://github.com/aspnet/AspNetCore/blob/master/src/Mvc/Mvc.Core/src/DependencyInjection/MvcCoreServiceCollectionExtensions.cs)


```C#
public static IMvcCoreBuilder AddMvcCore(this IServiceCollection services)
{
    if (services == null)
    {
        throw new ArgumentNullException(nameof(services));
    }

    var partManager = GetApplicationPartManager(services);
    services.TryAddSingleton(partManager);

    ConfigureDefaultFeatureProviders(partManager);
    ConfigureDefaultServices(services);
    AddMvcCoreServices(services);

    var builder = new MvcCoreBuilder(services, partManager);

    return builder;
}
```

```C#
private static ApplicationPartManager GetApplicationPartManager(IServiceCollection services)
{
    var manager = GetServiceFromCollection<ApplicationPartManager>(services);
    if (manager == null)
    {
        manager = new ApplicationPartManager();

        var environment = GetServiceFromCollection<IWebHostEnvironment>(services);
        var entryAssemblyName = environment?.ApplicationName;
        if (string.IsNullOrEmpty(entryAssemblyName))
        {
            return manager;
        }

        manager.PopulateDefaultParts(entryAssemblyName);
    }

    return manager;
}
```

ApplicationPartManager 部分源码

```C#
internal void PopulateDefaultParts(string entryAssemblyName)
{
    var assemblies = GetApplicationPartAssemblies(entryAssemblyName);

    var seenAssemblies = new HashSet<Assembly>();

    foreach (var assembly in assemblies)
    {
        if (!seenAssemblies.Add(assembly))
        {
            // "assemblies" may contain duplicate values, but we want unique ApplicationPart instances.
            // Note that we prefer using a HashSet over Distinct since the latter isn't
            // guaranteed to preserve the original ordering.
            continue;
        }

        var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
        foreach (var applicationPart in partFactory.GetApplicationParts(assembly))
        {
            ApplicationParts.Add(applicationPart);
        }
    }
}

private static IEnumerable<Assembly> GetApplicationPartAssemblies(string entryAssemblyName)
{
    var entryAssembly = Assembly.Load(new AssemblyName(entryAssemblyName));

    // Use ApplicationPartAttribute to get the closure of direct or transitive dependencies
    // that reference MVC.
    var assembliesFromAttributes = entryAssembly.GetCustomAttributes<ApplicationPartAttribute>()
        .Select(name => Assembly.Load(name.AssemblyName))
        .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal)
        .SelectMany(GetAsemblyClosure);

    // The SDK will not include the entry assembly as an application part. We'll explicitly list it
    // and have it appear before all other assemblies \ ApplicationParts.
    return GetAsemblyClosure(entryAssembly)
        .Concat(assembliesFromAttributes);
}

private static IEnumerable<Assembly> GetAsemblyClosure(Assembly assembly)
{
    yield return assembly;

    var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false)
        .OrderBy(assembly => assembly.FullName, StringComparer.Ordinal);

    foreach (var relatedAssembly in relatedAssemblies)
    {
        yield return relatedAssembly;
    }
}
```