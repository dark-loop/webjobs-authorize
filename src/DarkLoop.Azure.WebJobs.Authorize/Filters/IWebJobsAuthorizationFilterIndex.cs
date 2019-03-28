using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.WebJobs;

namespace DarkLoop.Azure.WebJobs.Authorize.Filters
{
    interface IWebJobsAuthorizationFilterIndex
    {
        IWebJobsAuthorizeFilter GetAuthorizationFilter(string functionName);

        void AddAuthorizationFilter(MethodInfo functionMethod, FunctionNameAttribute nameAttribute, IEnumerable<IAuthorizeData> authorizeData);
    }
}
