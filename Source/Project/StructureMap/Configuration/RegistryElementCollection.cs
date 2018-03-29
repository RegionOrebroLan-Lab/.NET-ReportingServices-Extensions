using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RegionOrebroLan.ReportingServices.StructureMap.Configuration
{
	[SuppressMessage("Microsoft.Design", "CA1010: Collections should implement generic interface.")]
	public class RegistryElementCollection : ConfigurationElementCollection, IEnumerable<IRegistryElement>
	{
		#region Methods

		protected override ConfigurationElement CreateNewElement()
		{
			return new RegistryElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			if(element == null)
				throw new ArgumentNullException(nameof(element));

			return ((RegistryElement) element).Type;
		}

		IEnumerator<IRegistryElement> IEnumerable<IRegistryElement>.GetEnumerator() => this.Cast<IRegistryElement>().GetEnumerator();

		#endregion
	}
}