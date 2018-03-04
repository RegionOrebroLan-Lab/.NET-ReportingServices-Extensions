using System.Diagnostics.CodeAnalysis;
using StructureMap;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	[SuppressMessage("Naming", "CA1716:Identifiers should not match keywords")]
	public static class Global
	{
		#region Fields

		private static volatile IContainer _container;
		private static readonly object _lockObject = new object();

		#endregion

		#region Properties

		public static IContainer Container
		{
			get
			{
				// ReSharper disable InvertIf
				if(_container == null)
				{
					lock(_lockObject)
					{
						if(_container == null)
							_container = new Container(new Registry());
					}
				}
				// ReSharper restore InvertIf

				return _container;
			}
			set
			{
				if(value == _container)
					return;

				lock(_lockObject)
				{
					_container = value;
				}
			}
		}

		#endregion
	}
}