using System.Threading.Tasks;
using DarkLoop.WebJobs.Authorize.Security;

namespace DarkLoop.WebJobs.Authorize.Filters
{
    interface IWebJobsAuthorizeFilter
    {
        Task AuthorizeAsync(WebJobAuthorizationContext context);
    }
}
