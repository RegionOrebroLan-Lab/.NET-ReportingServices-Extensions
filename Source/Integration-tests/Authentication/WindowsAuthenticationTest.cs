using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.ReportingServices.Authentication;

namespace RegionOrebroLan.ReportingServices.IntegrationTests.Authentication
{
	[TestClass]
	public class WindowsAuthenticationTest
	{
		#region Methods

		[TestMethod]
		public void Constructor_Test()
		{
			var windowsAuthentication = new WindowsAuthentication();

			var firstWindowsAuthenticationInternal = windowsAuthentication.WindowsAuthenticationInternal;
			var firstWebContext = windowsAuthentication.WebContext;

			windowsAuthentication = new WindowsAuthentication();

			var secondWindowsAuthenticationInternal = windowsAuthentication.WindowsAuthenticationInternal;
			var secondWebContext = windowsAuthentication.WebContext;

			Assert.IsFalse(ReferenceEquals(firstWindowsAuthenticationInternal, secondWindowsAuthenticationInternal));
			Assert.IsTrue(ReferenceEquals(firstWebContext, secondWebContext));
		}

		#endregion
	}
}