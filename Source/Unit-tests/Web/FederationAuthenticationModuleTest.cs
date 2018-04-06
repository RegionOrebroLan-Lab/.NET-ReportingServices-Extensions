﻿using System;
using System.Web;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
			return new FederationAuthenticationModule(Mock.Of<IFormsAuthentication>(), Mock.Of<IFormsAuthenticationTicketFactory>(), Mock.Of<ILog>(), Mock.Of<IRedirectInformationFactory>(), this.CreateWebFacade(url));
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

			webFacadeMock.Setup(webFacade => webFacade.Request).Returns(this.CreateHttpRequest(url));

			return webFacadeMock.Object;
		}

		[TestMethod]
		public void GetRedirectInformationRegardingSlash_IfTheLocalPathOfTheHttpRequestEndsWithASlash_ShouldReturnRedirectInformationWithRedirectSetToFalseAndUrlSetToNull()
		{
			var url = new Uri("https://server.local.net/ReportServer/?&rs%3ACommand=ListChildren");

			var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

			var redirectInformation = federationAuthenticationModule.GetRedirectInformationRegardingSlash();

			Assert.IsFalse(redirectInformation.Redirect);
			Assert.IsNull(redirectInformation.Url);
		}

		[TestMethod]
		public void GetRedirectInformationRegardingSlash_IfTheLocalPathOfTheHttpRequestHasAFileExtension_ShouldReturnRedirectInformationWithRedirectSetToFalseAndUrlSetToNull()
		{
			var url = new Uri("https://server.local.net/ReportServer.test?&rs%3ACommand=ListChildren");

			var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

			var redirectInformation = federationAuthenticationModule.GetRedirectInformationRegardingSlash();

			Assert.IsFalse(redirectInformation.Redirect);
			Assert.IsNull(redirectInformation.Url);
		}

		[TestMethod]
		public void GetRedirectInformationRegardingSlash_IfTheLocalPathOfTheHttpRequestHasNoFileExtensionAndDoesNotEndWithASlash_ShouldReturnRedirectInformationWithRedirectSetToTrueAndUrlSetToAnUrlWithTheLocalPathEndingWithASlash()
		{
			var url = new Uri("https://server.local.net/ReportServer?&rs%3ACommand=ListChildren");
			var expectedUrl = new Uri("https://server.local.net/ReportServer/?&rs%3ACommand=ListChildren");

			var federationAuthenticationModule = this.CreateFederationAuthenticationModule(url);

			var redirectInformation = federationAuthenticationModule.GetRedirectInformationRegardingSlash();

			Assert.IsTrue(redirectInformation.Redirect);
			Assert.AreEqual(expectedUrl, redirectInformation.Url);
		}

		#endregion
	}
}