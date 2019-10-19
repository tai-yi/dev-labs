using TemplateLib;

namespace Microsoft.Extensions.DependencyInjection{
     public static class ServiceCollectionExtensions{
         public static IServiceCollection AddTemplates(this IServiceCollection services){
             services.TryAddSingleton<IRazorViewTextRenderer, RazorViewTextRendererWrapper>();
         }
     }
}