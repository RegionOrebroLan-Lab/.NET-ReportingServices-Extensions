using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace RegionOrebroLan.ReportingServices.StructureMap.Configuration
{
	public class RegistryElement : ConfigurationElement, IRegistryElement
	{
		#region Fields

		private const string _typePropertyName = "type";

		#endregion

		#region Properties

		[ConfigurationProperty(_typePropertyName, IsKey = true, IsRequired = true)]
		[SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public virtual string Type
		{
			get => (string) this[_typePropertyName];
			set => this[_typePropertyName] = value;
		}

		#endregion
	}
}