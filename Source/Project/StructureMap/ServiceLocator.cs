using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	public class ServiceLocator : InversionOfControl.IServiceLocator
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

		public virtual T GetService<T>()
		{
			return this.Container.GetInstance<T>();
		}

		public virtual T GetService<T>(string key)
		{
			return this.Container.GetInstance<T>(key);
		}

		public virtual object GetService(Type serviceType)
		{
			return this.Container.GetInstance(serviceType);
		}

		public virtual object GetService(Type serviceType, string key)
		{
			return this.Container.GetInstance(serviceType, key);
		}

		public virtual IEnumerable<T> GetServices<T>()
		{
			return this.Container.GetAllInstances<T>();
		}

		public virtual IEnumerable<object> GetServices(Type serviceType)
		{
			return this.Container.GetAllInstances(serviceType).Cast<object>();
		}

		#endregion
	}
}