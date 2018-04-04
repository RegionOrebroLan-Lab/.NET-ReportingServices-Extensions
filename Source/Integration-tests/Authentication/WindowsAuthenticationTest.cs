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
			var firstWebFacade = windowsAuthentication.WebFacade;

			windowsAuthentication = new WindowsAuthentication();

			var secondWindowsAuthenticationInternal = windowsAuthentication.WindowsAuthenticationInternal;
			var secondWebFacade = windowsAuthentication.WebFacade;

			Assert.IsFalse(ReferenceEquals(firstWindowsAuthenticationInternal, secondWindowsAuthenticationInternal));
			Assert.IsTrue(ReferenceEquals(firstWebFacade, secondWebFacade));
		}

		#endregion
	}
}