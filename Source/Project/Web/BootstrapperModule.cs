using System.Web;
using RegionOrebroLan.ReportingServices.Initialization;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class BootstrapperModule : IHttpModule
	{
		#region Fields

		private static bool _initialized;
		private static readonly object _lock = new object();

		#endregion

		#region Properties

		protected internal virtual bool Initialized
		{
			get => _initialized;
			set => _initialized = value;
		}

		protected internal virtual object Lock => _lock;

		#endregion

		#region Methods

		public virtual void Dispose() { }

		public virtual void Init(HttpApplication context)
		{
			if(this.Initialized)
				return;

			lock(this.Lock)
			{
				if(this.Initialized)
					return;

				Bootstrapper.Start();

				this.Initialized = true;
			}
		}

		#endregion
	}
}