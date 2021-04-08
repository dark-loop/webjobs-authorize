# webjobs-authorize
Bringing AuthorizeAttribute Behavior to Azure Functions v2. For v3 compatibility use [functions-authorize](https://github.com/dark-loop/functions-authorize).

It hooks into .NET Core dependency injection container to enable authentication and authorization in the same way  ASP.NET Core does.

## License
This projects is open source and may be redistributed under the terms of the [Apache 2.0](http://opensource.org/licenses/Apache-2.0) license.

## Using the package
### Installing the package
`dotnet add package DarkLoop.Azure.WebJobs.Authorize`

### Setting up authentication
The goal is to utilize the same authentication framework provided for ASP.NET Core
```c#
using Microsoft.Azure.WebJobs.Hosting;
using MyFunctionAppNamespace;

[assembly: WebJobsStartup(typeof(Startup))]
namespace MyFunctionAppNamespace
{
  class Startup : IWebJobsStartup
  {
    public void Configure(IWebJobsBuilder builder)
    {
      builder
        .AddAuthentication(options =>
        {
          options.DefaultAuthenticationScheme = JwtBearerDefaults.AuthenticationScheme;
          options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddOpenIdConnect(options =>
        {
          options.ClientId = "<my-client-id>";
          // ... more options here
        })
        .AddJwtBearer(options =>
        {
          options.Audience = "<my-audience>";
          // ... more options here
        });

      builder
        .AddAuthorization(options =>
        {
          options.AddPolicy("OnlyAdmins", policyBuilder =>
          {
            // configure my policy requirements
          });
        });
    }
  }
}
```

No need to register the middleware the way we do for ASP.NET Core applications.

### Using the attribute
And now lets use `WebJobAuthorizeAttribute` the same way we use `AuthorizeAttribute` in our ASP.NET Core applications.
```C#
public class Functions
{
  [WebJobAuthorize]
  [FunctionName("get-record")]
  public async Task<IActionResult> GetRecord(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
    ILogger log)
  {
    var user = req.HttpContext.User;
    var record = GetUserData(user.Identity.Name);
    return new OkObjectResult(record);
  }

  [WebJobAuthorize(Policy = "OnlyAdmins")]
  [FunctionName("get-all-records")]
  public async Task<IActionResult>(
    [HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req,
    ILogger log)
  {
    var records = GetAllData();
    return new OkObjectResult(records);
  }
}
```
##

### Releases
[![Nuget](https://img.shields.io/nuget/v/DarkLoop.Azure.WebJobs.Authorize.svg)](https://www.nuget.org/packages/DarkLoop.Azure.WebJobs.Authorize)

### Builds
![master build status](https://dev.azure.com/darkloop/DarkLoop%20Core%20Library/_apis/build/status/Open%20Source/WebJobs%20Authorize%20-%20Pack?branchName=master)

