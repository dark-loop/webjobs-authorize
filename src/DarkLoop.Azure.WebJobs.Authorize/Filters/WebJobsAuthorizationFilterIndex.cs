using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.WebJobs;

namespace DarkLoop.Azure.WebJobs.Authorize.Filters
{
    class WebJobAuthorizationFilterIndex : IWebJobsAuthorizationFilterIndex
    {
        private readonly ConcurrentDictionary<string, IWebJobsAuthorizeFilter> _index =
            new ConcurrentDictionary<string, IWebJobsAuthorizeFilter>();
        private readonly IAuthorizationPolicyProvider _policyProvider;
        private readonly IAuthenticationSchemeProvider _schemeProvider;

        public WebJobAuthorizationFilterIndex(
            IAuthenticationSchemeProvider schemeProvider,
            IAuthorizationPolicyProvider policyProvider)
        {
            _schemeProvider = schemeProvider;
            _policyProvider = policyProvider;
        }

        public void AddAuthorizationFilter(MethodInfo functionMethod, FunctionNameAttribute nameAttribute, IEnumerable<IAuthorizeData> authorizeData)
        {
            if (functionMethod is null) throw new ArgumentNullException(nameof(functionMethod));
            if (authorizeData is null) throw new ArgumentNullException(nameof(authorizeData));

            var name = this.GetFunctionName(functionMethod, nameAttribute);
            var filter = new WebJobsAuthorizeFilter(_schemeProvider, _policyProvider, authorizeData);

            if (!this._index.TryAdd(name, filter))
            {
                throw new InvalidOperationException($"An authorization filter for function {name} has already been processed. Make sure function names are unique within your Functions App.");
            }
        }

        public IWebJobsAuthorizeFilter GetAuthorizationFilter(string functionName)
        {
            _index.TryGetValue(functionName, out var stored);

            return stored;
        }

        private string GetFunctionName(MethodInfo method, FunctionNameAttribute nameAttribute)
        {
            if (nameAttribute is null)
            {
                return $"{method.DeclaringType.Name}.{method.Name}";
            }
            else
            {
                return nameAttribute.Name;
            }
        }
    }
}
