using Microsoft.VisualStudio.TestTools.UnitTesting;
using RegionOrebroLan.ReportingServices.Authentication;

namespace RegionOrebroLan.ReportingServices.IntegrationTests.Authentication
{
	[TestClass]
	public class IdentityResolverFactoryTest
	{
		#region Methods

		[TestMethod]
		public void Create_IfTheConfigurationIsACDataElement_Test()
		{
			const string configuration = "<![CDATA[<system.identityModel><identityConfiguration><certificateValidation certificateValidationMode=\"PeerOrChainTrust\" /><securityTokenHandlers><clear /><add type=\"System.IdentityModel.Tokens.SamlSecurityTokenHandler, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><samlSecurityTokenRequirement issuerCertificateRevocationMode=\"Online\" issuerCertificateTrustedStoreLocation=\"LocalMachine\" issuerCertificateValidationMode=\"PeerOrChainTrust\" mapToWindows=\"true\" ><nameClaimType value=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\" /><roleClaimType value=\"schemas.microsoft.com/ws/2006/04/identity/claims/role\" /></samlSecurityTokenRequirement></add></securityTokenHandlers></identityConfiguration></system.identityModel>]]>";

			var identityResolver = (IdentityResolver) new IdentityResolverFactory().Create(configuration);

			Assert.AreEqual(8, identityResolver.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers.Count);
		}

		[TestMethod]
		public void Create_Test()
		{
			const string configuration = "<system.identityModel><identityConfiguration><certificateValidation certificateValidationMode=\"PeerOrChainTrust\" /><securityTokenHandlers><clear /><add type=\"System.IdentityModel.Tokens.SamlSecurityTokenHandler, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"><samlSecurityTokenRequirement issuerCertificateRevocationMode=\"Online\" issuerCertificateTrustedStoreLocation=\"LocalMachine\" issuerCertificateValidationMode=\"PeerOrChainTrust\" mapToWindows=\"true\" ><nameClaimType value=\"http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name\" /><roleClaimType value=\"schemas.microsoft.com/ws/2006/04/identity/claims/role\" /></samlSecurityTokenRequirement></add></securityTokenHandlers></identityConfiguration></system.identityModel>";

			var identityResolver = (IdentityResolver) new IdentityResolverFactory().Create(configuration);

			Assert.AreEqual(8, identityResolver.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers.Count);
		}

		#endregion
	}
}