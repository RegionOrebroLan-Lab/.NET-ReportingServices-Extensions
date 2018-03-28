using System;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.UnitTests.Web
{
	[TestClass]
	public class RedirectInformationFactoryTest
	{
		#region Methods

		protected internal virtual RedirectInformationFactory CreateRedirectInformationFactory(Uri url)
		{
			var uriBuilder = new UriBuilder(url);

			var applicationPath = "/" + url.LocalPath.Split('/').FirstOrDefault(item => !string.IsNullOrWhiteSpace(item));

			var queryString = HttpUtility.ParseQueryString(uriBuilder.Query);

			var httpRequestMock = new Mock<HttpRequestBase>();
			httpRequestMock.Setup(httpRequest => httpRequest.ApplicationPath).Returns(applicationPath);
			httpRequestMock.Setup(httpRequest => httpRequest.QueryString).Returns(queryString);
			httpRequestMock.Setup(httpRequest => httpRequest.Url).Returns(url);

			var webContextMock = new Mock<IWebContext>();
			webContextMock.Setup(webContext => webContext.HttpRequest).Returns(httpRequestMock.Object);

			return new RedirectInformationFactory(webContextMock.Object);
		}

		[TestMethod]
		public void Test()
		{
			var url = new Uri("https://company.net/ReportServer/?ReturnUrl=/ReportServer/localredirect?url=%252fReports%252f");

			var redirectInformationFactory = this.CreateRedirectInformationFactory(url);

			var redirectInformation = redirectInformationFactory.Create();

			Assert.IsNull(redirectInformation.Exception);
			Assert.IsTrue(redirectInformation.Redirect);
			Assert.AreEqual(new Uri("/Reports/", UriKind.Relative), redirectInformation.Url);
		}

		#endregion
	}
}