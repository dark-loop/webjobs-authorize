using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DarkLoop.Azure.WebJobs.Authorize.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DarkLoop.Azure.WebJobs.Authorize
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WebJobAuthorizeAttribute : FunctionInvocationFilterAttribute, IFunctionInvocationFilter, IAuthorizeData
    {
        public WebJobAuthorizeAttribute() { }

        public WebJobAuthorizeAttribute(string policy)
        {
            this.Policy = policy;
        }

        public string Policy { get; set; }

        public string Roles { get; set; }

        public string AuthenticationSchemes { get; set; }

        async Task IFunctionInvocationFilter.OnExecutingAsync(FunctionExecutingContext executingContext, CancellationToken cancellationToken)
        {
            if (!this.IsProcessed(executingContext))
            {
                var httpContext = this.GetHttpContext(executingContext);
                if (httpContext != null)
                {
                    await this.AuthorizeRequest(executingContext, httpContext);
                }
            }
        }

        private bool IsProcessed(FunctionExecutingContext context)
        {
            const string valueKey = "__AuthZProcessed";

            if (!context.Properties.TryGetValue(valueKey, out var value))
            {
                context.Properties[valueKey] = true;
                return false;
            }

            return (bool)value;
        }

        private HttpContext GetHttpContext(FunctionExecutingContext context)
        {
            var requestOrMessage = context.Arguments.Values.FirstOrDefault(x => x is HttpRequest || x is HttpRequestMessage);

            if (requestOrMessage is HttpRequest request)
            {
                return request.HttpContext;
            }
            else if (requestOrMessage is HttpRequestMessage message)
            {
                return message.Properties[nameof(HttpContext)] as HttpContext;
            }
            else
            {
                return null;
            }
        }

        private async Task AuthorizeRequest(FunctionExecutingContext functionContext, HttpContext httpContext)
        {
            var handler = httpContext.RequestServices.GetRequiredService<IWebJobsHttpAuthorizationHandler>();
            await handler.OnAuthorizingFunctionInstance(functionContext, httpContext);
        }
    }
}
