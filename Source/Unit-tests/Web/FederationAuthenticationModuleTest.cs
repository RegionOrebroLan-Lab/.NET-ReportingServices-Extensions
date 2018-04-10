using System;
using System.Web;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.ReportingServices.Security.Principal;
using RegionOrebroLan.ReportingServices.Web;
using RegionOrebroLan.ReportingServices.Web.Security;

namespace RegionOrebroLan.ReportingServices.UnitTests.Web
{
	[TestClass]
	public class FederationAuthenticationModuleTest
	{
		#region Methods

		protected internal virtual FederationAuthenticationModule CreateFederationAuthenticationModule(Uri url)
		{
			return new FederationAuthenticationModule(Mock.Of<IFormsAuthentication>(), Mock.Of<IFormsAuthenticationTicketFactory>(), Mock.Of<ILog>(), Mock.Of<IRedirectInformationFactory>(), this.CreateWebFacade(url), Mock.Of<IWindowsFederationIdentityFactory>());
		}

		protected internal virtual HttpRequestBase CreateHttpRequest(Uri url)
		{
			var httpRequestMock = new Mock<HttpRequestBase>();

			httpRequestMock.Setup(httpRequest => httpRequest.Url).Returns(url);

			return httpRequestMock.Object;
		}

		protected internal virtual IWebFacade CreateWebFacade(Uri url)
		{
			var webFacadeMock = new Mock<IWebFacade>();

			var httpContextMock = new Mock<HttpContextBase>();
			httpContextMock.Setup(httpContext => httpContext.ApplicationInstance).Returns(new HttpApplication());

			webFacadeMock.Setup(webFacade => webFacade.Context).Returns(httpContextMock.Object);
			webFacadeMock.Setup(webFacade => webFacade.Request).Returns(this.CreateHttpRequest(url));
			webFacadeMock.Setup(webFacade => webFacade.Response).Returns(Mock.Of<HttpResponseBase>());

			return webFacadeMock.Object;
		}

		[TestMethod]
		public void RedirectIfTrailingSlashIsMissing_IfTheLocalPathOfTheHttpRequestEndsWithASlash_ShouldReturnFalse()
		{
			var url = new Uri("https://server.local.net/ReportServer/?&rs%3ACommand=ListChildren");

			var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

			Assert.IsFalse(federationAuthenticationModule.RedirectIfTrailingSlashIsMissing());
		}

		[TestMethod]
		public void RedirectIfTrailingSlashIsMissing_IfTheLocalPathOfTheHttpRequestHasAFileExtension_ShouldReturnFalse()
		{
			var url = new Uri("https://server.local.net/ReportServer.test?&rs%3ACommand=ListChildren");

			var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

			Assert.IsFalse(federationAuthenticationModule.RedirectIfTrailingSlashIsMissing());
		}

		#endregion

		//[TestMethod]
		//public void RedirectIfTrailingSlashIsMissing_IfTheLocalPathOfTheHttpRequestHasNoFileExtensionAndDoesNotEndWithASlash_ShouldReturnTrue()
		//{
		//	var url = new Uri("https://server.local.net/ReportServer?&rs%3ACommand=ListChildren");

		//	var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

		//	Assert.IsTrue(federationAuthenticationModule.RedirectIfTrailingSlashIsMissing());
		//}
	}
}