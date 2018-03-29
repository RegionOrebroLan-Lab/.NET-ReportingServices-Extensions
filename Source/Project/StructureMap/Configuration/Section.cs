using System.Configuration;

namespace RegionOrebroLan.ReportingServices.StructureMap.Configuration
{
	public class Section : ConfigurationSection
	{
		#region Fields

		private const string _registriesPropertyName = "registries";

		#endregion

		#region Properties

		[ConfigurationProperty(_registriesPropertyName)]
		public virtual RegistryElementCollection Registries => (RegistryElementCollection) this[_registriesPropertyName];

		#endregion
	}
}