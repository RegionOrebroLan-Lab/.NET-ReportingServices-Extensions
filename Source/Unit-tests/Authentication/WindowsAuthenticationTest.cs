using log4net;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.ReportingServices.Authentication;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.UnitTests.Authentication
{
	[TestClass]
	public class WindowsAuthenticationTest
	{
		#region Methods

		protected internal virtual WindowsAuthentication CreateWindowsAuthentication()
		{
			return new WindowsAuthentication(Mock.Of<IIdentityResolver>(), Mock.Of<ILog>(), Mock.Of<IWebFacade>(), Mock.Of<IWindowsAuthenticationExtension2>());
		}

		[TestMethod]
		public void GetMaskedValue_IfTheValueParameterIsAnEmptyString_ShouldReturnAnEmptyString()
		{
			Assert.AreEqual(string.Empty, this.CreateWindowsAuthentication().GetMaskedValue(string.Empty));
		}

		[TestMethod]
		public void GetMaskedValue_IfTheValueParameterIsNull_ShouldReturnNull()
		{
			Assert.IsNull(this.CreateWindowsAuthentication().GetMaskedValue(null));
		}

		[TestMethod]
		public void GetMaskedValue_ShouldReturnAMaskedValueOfTheValueParameter_WhereEachCharacterIsReplacedWithAWildcard()
		{
			Assert.AreEqual("*", this.CreateWindowsAuthentication().GetMaskedValue(" "));

			Assert.AreEqual("*****", this.CreateWindowsAuthentication().GetMaskedValue("     "));

			Assert.AreEqual("***********", this.CreateWindowsAuthentication().GetMaskedValue(" a c d e f "));
		}

		#endregion
	}
}