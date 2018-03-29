using RegionOrebroLan.ReportingServices.StructureMap;
using StructureMapServiceLocator = RegionOrebroLan.ReportingServices.StructureMap.ServiceLocator;

namespace RegionOrebroLan.ReportingServices.InversionOfControl
{
	public static class ServiceLocator
	{
		#region Fields

		private static volatile IServiceLocator _instance;
		private static readonly object _lock = new object();

		#endregion

		#region Properties

		public static IServiceLocator Instance
		{
			get
			{
				// ReSharper disable InvertIf
				if(_instance == null)
				{
					lock(_lock)
					{
						if(_instance == null)
							_instance = new StructureMapServiceLocator(Global.Container);
					}
				}
				// ReSharper restore InvertIf

				return _instance;
			}
			set
			{
				if(value == _instance)
					return;

				lock(_lock)
				{
					_instance = value;
				}
			}
		}

		#endregion
	}
}