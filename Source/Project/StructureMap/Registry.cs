using System;
using StructureMap.Configuration.DSL;
using StructureMapRegistry = StructureMap.Configuration.DSL.Registry;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	public class Registry : StructureMapRegistry
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
		}

		#endregion
	}
}