using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace JMS.Service.ServiceContracts
{
    public interface IRazorViewToStringRenderer
    {
        Task<string> RenderViewToStringAsync<TModel>(string viewName, TModel model);
    }
}

