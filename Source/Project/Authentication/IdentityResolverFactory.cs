using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Xml;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class IdentityResolverFactory : IIdentityResolverFactory
	{
		#region Methods

		[SuppressMessage("Microsoft.Security", "CA3075:InsecureDTDProcessing")]
		public virtual IIdentityResolver Create(string configuration)
		{
			using(var stringReader = new StringReader(configuration))
			{
				using(var xmlReader = XmlReader.Create(stringReader))
				{
					var systemIdentityModelSection = new SystemIdentityModelSection();

					// ReSharper disable PossibleNullReferenceException
					typeof(ConfigurationSection).GetMethod("DeserializeSection", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(systemIdentityModelSection, new object[] {xmlReader});
					// ReSharper restore PossibleNullReferenceException

					var identityConfigurationElement = systemIdentityModelSection.IdentityConfigurationElements.GetElement(string.Empty);
					var identityConfiguration = new IdentityConfiguration(false);

					// ReSharper disable PossibleNullReferenceException
					typeof(IdentityConfiguration).GetMethod("LoadConfiguration", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(identityConfiguration, new object[] {identityConfigurationElement});
					// ReSharper restore PossibleNullReferenceException

					// We need to set <clear /> as the first element in the configuration in RSReportServer.config
					/*
						<Configuration>
							...
							<Extensions>
								...
								<Authentication>
									<Extension Name="Windows" Type="RegionOrebroLan.ReportingServices.Authentication.WindowsAuthentication, RegionOrebroLan.ReportingServices">
										<Configuration>
											<system.identityModel>
												<identityConfiguration>
													<certificateValidation certificateValidationMode="PeerOrChainTrust" />
													<securityTokenHandlers>
														
														<clear /><!-- Important -->
														
														<add type="RegionOrebroLan.IdentityModel.Tokens.SamlImpersonatableSecurityTokenHandler, RegionOrebroLan.IdentityModel, Version=0.0.0.0, Culture=neutral, PublicKeyToken=520b099ae7bbdead">
															<samlSecurityTokenRequirement
																issuerCertificateRevocationMode="Online"
																issuerCertificateTrustedStoreLocation="LocalMachine"
																issuerCertificateValidationMode="PeerOrChainTrust"
																mapToWindows="true"
															>
																<nameClaimType value="http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" />
																<roleClaimType value="schemas.microsoft.com/ws/2006/04/identity/claims/role" />
															</samlSecurityTokenRequirement>
														</add>
													</securityTokenHandlers>
												</identityConfiguration>
											</system.identityModel>
										</Configuration>
									</Extension>
								</Authentication>
								...
							</Extensions>
							...
						</Configuration>
					 */
					var configuredSecurityTokenHandlers = new List<SecurityTokenHandler>();

					// Save configured handlers temporarily.
					foreach(var securityTokenHandler in identityConfiguration.SecurityTokenHandlers)
					{
						configuredSecurityTokenHandlers.Add(securityTokenHandler);
					}

					// Clear configured handlers, we replace them later.
					identityConfiguration.SecurityTokenHandlers.Clear();

					// Add default handlers
					foreach(var securityTokenHandler in new IdentityConfiguration(false).SecurityTokenHandlers)
					{
						identityConfiguration.SecurityTokenHandlers.Add(securityTokenHandler);
					}

					// Replace with configured handlers
					foreach(var configuredSecurityTokenHandler in configuredSecurityTokenHandlers)
					{
						identityConfiguration.SecurityTokenHandlers.AddOrReplace(configuredSecurityTokenHandler);
					}

					var federationConfiguration = new FederationConfiguration(false)
					{
						IdentityConfiguration = identityConfiguration
					};

					return new IdentityResolver {FederationConfiguration = federationConfiguration};
				}
			}
		}

		#endregion
	}
}