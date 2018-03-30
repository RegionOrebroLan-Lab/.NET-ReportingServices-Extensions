using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using log4net;
using log4net.Config;
using RegionOrebroLan.ReportingServices.StructureMap;
using RegionOrebroLan.ReportingServices.StructureMap.Configuration;

namespace RegionOrebroLan.ReportingServices.Initialization
{
	public class Bootstrapper
	{
		#region Constructors

		public Bootstrapper(ILog log)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
		}

		#endregion

		#region Properties

		protected internal virtual ILog Log { get; }

		#endregion

		#region Methods

		[SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		public virtual void Bootstrap()
		{
			if(this.Log.IsDebugEnabled)
				this.Log.Debug("Bootstrap started.");

			Global.Container.Configure(configuration =>
			{
				foreach(IRegistryElement registryElement in ((Section) ConfigurationManager.GetSection("structureMap")).Registries)
				{
					// ReSharper disable AssignNullToNotNullAttribute
					configuration.AddRegistry((global::StructureMap.Configuration.DSL.Registry) Activator.CreateInstance(Type.GetType(registryElement.Type, true)));
					// ReSharper restore AssignNullToNotNullAttribute
				}
			});

			if(this.Log.IsDebugEnabled)
				this.Log.Debug("Bootstrap finished.");
		}

		public static void Start()
		{
			if(!LogManager.GetRepository().Configured)
				XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));

			new Bootstrapper(LogManager.GetLogger(typeof(Bootstrapper))).Bootstrap();
		}

		#endregion
	}
}