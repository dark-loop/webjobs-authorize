using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DarkLoop.WebJobs.Authorize.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DarkLoop.WebJobs.Authorize.Filters
{
    internal class WebJobsAuthorizeFilter : IWebJobsAuthorizeFilter
    {
        private const string AuthInvokedKey = "__WebJobAuthInvoked";

        private AuthorizationPolicy _effectivePolicy;

        public IEnumerable<IAuthorizeData> AuthorizeData { get; }

        public IAuthenticationSchemeProvider SchemeProvider { get; }

        public IAuthorizationPolicyProvider PolicyProvider { get; }

        public AuthorizationPolicy Policy { get; }

        public WebJobsAuthorizeFilter(IEnumerable<IAuthorizeData> authorizeData)
        {
            this.AuthorizeData = authorizeData;
        }

        public WebJobsAuthorizeFilter(
            IAuthenticationSchemeProvider schemeProvider,
            IAuthorizationPolicyProvider policyProvider,
            IEnumerable<IAuthorizeData> authorizeData)
            : this(authorizeData)
        {
            this.SchemeProvider = schemeProvider;
            this.PolicyProvider = policyProvider;
        }

        public WebJobsAuthorizeFilter(string policy)
            : this(new[] { new WebJobAuthorizeAttribute(policy) }) { }

        public async Task AuthorizeAsync(WebJobAuthorizationContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Items.ContainsKey(AuthInvokedKey))
            {
                return;
            }

            var effectivePolicy = await this.ComputePolicyAsync();

            if (effectivePolicy is null)
            {
                return;
            }

            var httpContext = context.HttpContext;
            await this.AuthenticateRequestAsync(context);
            var evaluator = httpContext.RequestServices.GetRequiredService<IPolicyEvaluator>();
            var authenticateResult = await evaluator.AuthenticateAsync(effectivePolicy, context.HttpContext);
            var authorizeResult = await evaluator.AuthorizeAsync(effectivePolicy, authenticateResult, context.HttpContext, context);

            if (authorizeResult.Challenged)
            {
                context.Result = new ChallengeResult(effectivePolicy.AuthenticationSchemes.ToArray());
            }
            else if (authorizeResult.Forbidden)
            {
                context.Result = new ForbidResult(effectivePolicy.AuthenticationSchemes.ToArray());
            }

        }

        private async Task<AuthenticateResult> AuthenticateRequestAsync(WebJobAuthorizationContext context)
        {
            var httpContext = context.HttpContext;
            var handlers = httpContext.RequestServices.GetService<IAuthenticationHandlerProvider>();

            foreach (var scheme in await this.SchemeProvider.GetRequestHandlerSchemesAsync())
            {
                var handler = await handlers.GetHandlerAsync(httpContext, scheme.Name) as IAuthenticationRequestHandler;
                if (handler != null)
                {
                    var result = await handler.AuthenticateAsync();
                    if (result.Succeeded)
                    {
                        httpContext.User = result.Principal;
                        return result;
                    }
                }
            }

            var defaultAuthenticate = await this.SchemeProvider.GetDefaultAuthenticateSchemeAsync();
            if (defaultAuthenticate != null)
            {
                var result = await httpContext.AuthenticateAsync(defaultAuthenticate.Name);
                if (result?.Principal != null)
                {
                    httpContext.User = result.Principal;
                    return result;
                }
            }

            return AuthenticateResult.NoResult();
        }

        private Task<AuthorizationPolicy> ComputePolicyAsync()
        {
            if (this.Policy != null)
            {
                return Task.FromResult(this.Policy);
            }

            if (this.PolicyProvider == null)
            {
                throw new InvalidOperationException("Policy cannot be created.");
            }

            return AuthorizationPolicy.CombineAsync(this.PolicyProvider, this.AuthorizeData);
        }
    }
}
