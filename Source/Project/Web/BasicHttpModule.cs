using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public abstract class BasicHttpModule : IHttpModule
	{
		#region Methods

		[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { }

		public virtual void Init(HttpApplication context)
		{
			this.Initialize((HttpApplicationWrapper) context);
		}

		public abstract void Initialize(IHttpApplication application);

		#endregion
	}
}