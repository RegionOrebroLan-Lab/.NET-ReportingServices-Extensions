using System;
using System.Web;
using log4net;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class ErrorHandlerModule : IHttpModule
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(ErrorHandlerModule));

		#endregion

		#region Constructors

		public ErrorHandlerModule() : this(_log) { }

		public ErrorHandlerModule(ILog log)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
		}

		#endregion

		#region Properties

		protected internal virtual ILog Log { get; }

		#endregion

		#region Methods

		public virtual void Dispose() { }

		public virtual void Init(HttpApplication context)
		{
			context.Error += this.OnError;
		}

		protected internal virtual void OnError(object sender, EventArgs e)
		{
			if(!this.Log.IsErrorEnabled)
				return;

			if(!(sender is HttpApplication httpApplication))
				return;

			var exception = httpApplication.Server.GetLastError();

			if(exception != null)
				this.Log.Error(exception);
		}

		#endregion
	}
}