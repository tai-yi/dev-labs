using Fs.Dayend.Wf.Templating.Internal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.ObjectPool;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace TemplateLib
{
    public class RazorViewTextRendererWrapper : IRazorViewTextRenderer
    {
        private Lazy<IServiceProvider> _internalServiceProvider;

        public Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model)
        {
            EnsureInternalServiceProviderLoad();
            using var scope = _internalServiceProvider.Value.CreateScope();
            var render = scope.ServiceProvider.GetRequiredService<IRazorViewTextRenderer>();
            return render.RenderViewToStringAsync(viewName, model);
        }

        private void EnsureInternalServiceProviderLoad()
        {
            if (_internalServiceProvider == null)
                _internalServiceProvider = new Lazy<IServiceProvider>(() =>
                {
                    var services = new ServiceCollection();
                    var assembly = typeof(RazorViewTextRendererWrapper).Assembly;
                    var fileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
                    services.AddSingleton<IWebHostEnvironment>(new RazorHostEnvironment
                    {
                        ApplicationName = assembly.GetName().Name,
                        ContentRootFileProvider = fileProvider,
                    });
                    var diagnosticSource = new DiagnosticListener("Microsoft.AspNetCore");
                    services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
                    services.AddSingleton<DiagnosticSource>(diagnosticSource);
                    services.AddSingleton(diagnosticSource);
                    services.AddLogging();
                    services.AddMvcCore().AddRazorPages();
                    services.AddTransient<IRazorViewTextRenderer, RazorViewToStringRenderer>();
                    return services.BuildServiceProvider();
                }, true);
        }
        
        private class RazorHostEnvironment : HostingEnvironment, IWebHostEnvironment
        {
            public IFileProvider WebRootFileProvider { get; set; }
            public string WebRootPath { get; set; }
        }
    }
}
