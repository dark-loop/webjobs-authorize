using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace DarkLoop.Azure.WebJobs.Authorize
{
    [Extension("WebJobsAuthorize")]
    class WebJobsAuthExtension : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {

        }
    }
}
