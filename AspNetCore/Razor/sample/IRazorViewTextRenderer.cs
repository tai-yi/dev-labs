using System.Threading.Tasks;

namespace TemplateLib
{
    public interface IRazorViewTextRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}
