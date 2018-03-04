using System;
using System.Collections.Generic;

namespace RegionOrebroLan.ReportingServices.InversionOfControl
{
	public interface IServiceLocator : IServiceProvider
	{
		#region Methods

		T GetService<T>();
		T GetService<T>(string key);
		object GetService(Type serviceType, string key);
		IEnumerable<T> GetServices<T>();
		IEnumerable<object> GetServices(Type serviceType);

		#endregion
	}
}