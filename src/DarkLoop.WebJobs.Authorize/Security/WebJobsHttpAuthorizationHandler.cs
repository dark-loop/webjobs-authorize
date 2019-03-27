using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DarkLoop.WebJobs.Authorize.Filters;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Host;

namespace DarkLoop.WebJobs.Authorize.Security
{
    internal class WebJobsHttpAuthorizationHandler : IWebJobsHttpAuthorizationHandler
    {
        private readonly IWebJobsAuthorizationFilterIndex _filtersIndex;

        public WebJobsHttpAuthorizationHandler(IWebJobsAuthorizationFilterIndex filtersIndex)
        {
            _filtersIndex = filtersIndex ?? throw new ArgumentNullException(nameof(filtersIndex));
        }

        public async Task OnAuthorizingFunctionInstance(FunctionExecutingContext functionContext, HttpContext httpContext)
        {
            if (functionContext is null) throw new ArgumentNullException(nameof(functionContext));
            if (httpContext is null) throw new ArgumentNullException(nameof(httpContext));

            var filter = _filtersIndex.GetAuthorizationFilter(functionContext.FunctionName);

            if (filter is null) return;

            var context = new WebJobAuthorizationContext(httpContext);

            await filter.AuthorizeAsync(context);

            if (context.Result is ChallengeResult challenge)
            {
                if (challenge.AuthenticationSchemes != null && challenge.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in challenge.AuthenticationSchemes)
                    {
                        await httpContext.ChallengeAsync(scheme);
                    }
                }
                else
                {
                    await httpContext.ChallengeAsync();
                }

                SetResponse("Unauthorized", httpContext.Response);
                BombFunctionInstance((int)HttpStatusCode.Unauthorized);
            }

            if (context.Result is ForbidResult forbid)
            {
                if (forbid.AuthenticationSchemes != null && forbid.AuthenticationSchemes.Count > 0)
                {
                    foreach (var scheme in forbid.AuthenticationSchemes)
                    {
                        await httpContext.ForbidAsync(scheme);
                    }
                }
                else
                {
                    await httpContext.ForbidAsync();
                }

                SetResponse("Forbidden", httpContext.Response);
                BombFunctionInstance((int)HttpStatusCode.Forbidden);
            }
        }

        private void SetResponse(string message, HttpResponse response)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            response.Headers.Add("Content-Type", "application/text");
            response.Headers.Add("Content-Length", bytes.Length.ToString());
            response.Body.Write(bytes, 0, bytes.Length);
            response.Body.Flush();
            response.Body.Close();
        }

        private void BombFunctionInstance(int status)
        {
            throw new Exception(
                $"{status} Authorization error encountered. This is the only way to stop function execution. The correct status has been communicated to caller");
        }
    }
}
