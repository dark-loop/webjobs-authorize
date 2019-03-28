using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace DarkLoop.WebJobs.Authorize
{
    [Extension("WebJobsAuthorize")]
    class WebJobsAuthExtension : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {

        }
    }
}
