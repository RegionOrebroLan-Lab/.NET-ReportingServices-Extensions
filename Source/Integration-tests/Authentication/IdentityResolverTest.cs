using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.ReportingServices.Authentication;
using RegionOrebroLan.ReportingServices.Web.Security;

namespace RegionOrebroLan.ReportingServices.IntegrationTests.Authentication
{
	[TestClass]
	public class IdentityResolverTest
	{
		#region Properties

		protected internal virtual IFormsAuthentication FormsAuthentication { get; } = new FormsAuthenticationWrapper();

		#endregion

		#region Methods

		protected internal virtual IdentityResolver CreateDeaultIdentityResolver()
		{
			return new IdentityResolver(this.FormsAuthentication);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetIdentity_IfThereAreInvalidCookies_ShouldThrowAnInvalidOperationException()
		{
			var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{this.FormsAuthentication.CookieName, "Invalid-value"}
			};

			var identityResolver = this.CreateDeaultIdentityResolver();

			var identity = identityResolver.GetIdentity(cookies);

			Assert.IsNull(identity);
		}

		[TestMethod]
		public void GetIdentity_IfThereAreNoRelevantCookies_ShouldReturnNull()
		{
			var identityResolver = this.CreateDeaultIdentityResolver();

			var identity = identityResolver.GetIdentity(new Dictionary<string, string>());

			Assert.IsNull(identity);
		}

		#endregion
	}
}