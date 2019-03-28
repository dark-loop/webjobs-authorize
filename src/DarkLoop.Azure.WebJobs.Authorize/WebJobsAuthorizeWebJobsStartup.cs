using DarkLoop.Azure.WebJobs.Authorize;
using DarkLoop.Azure.WebJobs.Authorize.Bindings;
using DarkLoop.Azure.WebJobs.Authorize.Filters;
using DarkLoop.Azure.WebJobs.Authorize.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsAuthorizeWebJobsStartup))]
namespace DarkLoop.Azure.WebJobs.Authorize
{
    class WebJobsAuthorizeWebJobsStartup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddSingleton<IBindingProvider, WebJobsAuthorizeBindingProvider>();
            builder.Services.AddSingleton<IWebJobsAuthorizationFilterIndex, WebJobAuthorizationFilterIndex>();
            builder.Services.AddSingleton<IWebJobsHttpAuthorizationHandler, WebJobsHttpAuthorizationHandler>();
            builder.AddExtension<WebJobsAuthExtension>();
        }
    }
}
