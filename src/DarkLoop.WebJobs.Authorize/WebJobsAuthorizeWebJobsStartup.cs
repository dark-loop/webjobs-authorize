using DarkLoop.WebJobs.Authorize;
using DarkLoop.WebJobs.Authorize.Bindings;
using DarkLoop.WebJobs.Authorize.Filters;
using DarkLoop.WebJobs.Authorize.Security;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(WebJobsAuthorizeWebJobsStartup))]
namespace DarkLoop.WebJobs.Authorize
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
