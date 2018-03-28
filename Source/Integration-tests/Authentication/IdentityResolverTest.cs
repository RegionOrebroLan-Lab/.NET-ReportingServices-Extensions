using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.ReportingServices.Authentication;

namespace RegionOrebroLan.ReportingServices.IntegrationTests.Authentication
{
	[TestClass]
	public class IdentityResolverTest
	{
		#region Methods

		protected internal virtual IdentityResolver CreateDeaultIdentityResolver()
		{
			const string configuration = "<system.identityModel><identityConfiguration><certificateValidation certificateValidationMode=\"PeerOrChainTrust\" /><securityTokenHandlers><clear /><add type=\"System.IdentityModel.Tokens.SamlSecurityTokenHandler, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><samlSecurityTokenRequirement issuerCertificateRevocationMode=\"Online\" issuerCertificateTrustedStoreLocation=\"LocalMachine\" issuerCertificateValidationMode=\"PeerOrChainTrust\" mapToWindows=\"true\" ><nameClaimType value=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\" /><roleClaimType value=\"schemas.microsoft.com/ws/2006/04/identity/claims/role\" /></samlSecurityTokenRequirement></add></securityTokenHandlers></identityConfiguration></system.identityModel>";

			return (IdentityResolver) new IdentityResolverFactory().Create(configuration);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidOperationException))]
		public void GetIdentity_IfThereAreInvalidCookies_ShouldThrowAnInvalidOperationException()
		{
			var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{"FedAuth", "Invalid-value"},
				{"FedAuth1", "Invalid-value"}
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