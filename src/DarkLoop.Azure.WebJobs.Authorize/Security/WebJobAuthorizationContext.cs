using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DarkLoop.Azure.WebJobs.Authorize.Security
{
    internal class WebJobAuthorizationContext
    {
        public WebJobAuthorizationContext(HttpContext httpContext)
        {
            HttpContext = httpContext;
        }

        public HttpContext HttpContext { get; }

        public IActionResult Result { get; internal set; }
    }
}
