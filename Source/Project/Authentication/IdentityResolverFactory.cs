using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Configuration;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class IdentityResolverFactory : IIdentityResolverFactory
	{
		#region Methods

		[SuppressMessage("Microsoft.Security", "CA3075:InsecureDTDProcessing")]
		public virtual IIdentityResolver Create(string configuration)
		{
			var xmlDocument = new XmlDocument();

			xmlDocument.LoadXml("<root>" + configuration + "</root>");

			// ReSharper disable PossibleNullReferenceException
			var element = xmlDocument.DocumentElement.ChildNodes[0];
			// ReSharper restore PossibleNullReferenceException

			configuration = element is XmlCDataSection ? element.InnerText : element.OuterXml;

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

					var federationConfiguration = (FederationConfiguration) FormatterServices.GetUninitializedObject(typeof(FederationConfiguration));
					federationConfiguration.IdentityConfiguration = identityConfiguration;

					return new IdentityResolver {FederationConfiguration = federationConfiguration};
				}
			}
		}

		#endregion
	}
}