using System.Threading.Tasks;
using DarkLoop.Azure.WebJobs.Authorize.Security;

namespace DarkLoop.Azure.WebJobs.Authorize.Filters
{
    interface IWebJobsAuthorizeFilter
    {
        Task AuthorizeAsync(WebJobAuthorizationContext context);
    }
}
