using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunctionApp1.Application
{

    public interface IRazorService
    {
        Task<string> RenderViewToStringAsync<TModel>(TModel model);
        Task<string> RenderViewToStringAsync<TModel>(TModel model, string viewName);
    }
}
