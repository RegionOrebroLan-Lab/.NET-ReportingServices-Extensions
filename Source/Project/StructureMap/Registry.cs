using System;
using Microsoft.ReportingServices.Authentication;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Web;
using StructureMap.Configuration.DSL;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	public class Registry : global::StructureMap.Configuration.DSL.Registry
	{
		#region Constructors

		public Registry()
		{
			Register(this);
		}

		#endregion

		#region Methods

		public static void Register(IProfileRegistry registry)
		{
			if(registry == null)
				throw new ArgumentNullException(nameof(registry));

			registry.For<IWebContext>().Singleton().Use<WebContext>();
			registry.For<IWindowsAuthenticationExtension2>().Singleton().Use<WindowsAuthentication>();
		}

		#endregion
	}
}