using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DarkLoop.Azure.WebJobs.Authorize.Filters;
using DarkLoop.Azure.WebJobs.Authorize.Security;
using DarkLoop.Azure.WebJobs.Authorize.Tests.Mocks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DarkLoop.Azure.WebJobs.Authorize.Tests
{
    [TestClass]
    public class WebJobsHttpAuthorizationHandlerShould
    {
        [TestMethod]
        public void ThrowExceptionWhenFiltersIndexIsNull()
        {
            var action = new Action(() => new WebJobsHttpAuthorizationHandler(null));
            action.Should().Throw<ArgumentNullException>("No null param is allowed");
        }

        [TestMethod]
        public void NotThrowExceptionWhenFiltersIndexIsNotNull()
        {
            var index = new Mock<IWebJobsAuthorizationFilterIndex>();
            var action = new Action(() => new WebJobsHttpAuthorizationHandler(index.Object));
            action.Should().NotThrow<ArgumentNullException>("index is expected");
        }

        [TestMethod]
        public void ThrowWhenOnAuthorizingFunctionContextParamIsNull()
        {
            var index = new Mock<IWebJobsAuthorizationFilterIndex>();
            var handler = new WebJobsHttpAuthorizationHandler(index.Object);
            var action = new Func<Task>(async () => await handler.OnAuthorizingFunctionInstance(null, null));
            action.Should().Throw<ArgumentNullException>("functionContext is expected not to be null");
        }
    }
}
