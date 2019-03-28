using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;

namespace DarkLoop.WebJobs.Authorize.Security
{
    internal interface IWebJobsHttpAuthorizationHandler
    {
        Task OnAuthorizingFunctionInstance(FunctionExecutingContext functionContext, HttpContext httpContext);
    }
}
