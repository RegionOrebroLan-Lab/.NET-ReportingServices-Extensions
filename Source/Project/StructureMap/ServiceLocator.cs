using System;
using System.Collections.Generic;
using System.Linq;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using StructureMap;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	public class ServiceLocator : IServiceLocator
	{
		#region Constructors

		public ServiceLocator(IContainer container)
		{
			this.Container = container ?? throw new ArgumentNullException(nameof(container));
		}

		#endregion

		#region Properties

		protected internal virtual IContainer Container { get; }

		#endregion

		#region Methods

		public virtual object GetService(Type serviceType) => this.Container.GetInstance(serviceType);
		public virtual object GetService(string key, Type serviceType) => this.Container.GetInstance(serviceType, key);
		public virtual T GetService<T>() => this.Container.GetInstance<T>();
		public virtual T GetService<T>(string key) => this.Container.GetInstance<T>(key);
		public virtual IEnumerable<T> GetServices<T>() => this.Container.GetAllInstances<T>();
		public virtual IEnumerable<object> GetServices(Type serviceType) => this.Container.GetAllInstances(serviceType).Cast<object>();

		#endregion
	}
}