using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using log4net;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class SessionAuthenticationModule : System.IdentityModel.Services.SessionAuthenticationModule
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationModule));
		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public SessionAuthenticationModule() : this(_log, _webContext) { }

		public SessionAuthenticationModule(ILog log, IWebContext webContext)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual ILog Log { get; }
		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		protected internal virtual void LogDebugIfEnabled(string method)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			var user = this.WebContext.HttpContext.User;
			var userName = user == null ? "NULL" : (user.Identity.IsAuthenticated ? user.Identity.Name : "Anonymous");

			this.Log.Debug(string.Format(CultureInfo.InvariantCulture, "SessionAuthenticationModule - {0}: user = \"{1}\", url = {2}", method, userName, this.WebContext.HttpRequest.Url));
		}

		[SuppressMessage("Microsoft.Naming", "CA1725: Parameter names should match base declaration")]
		protected override void OnAuthenticateRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("Entering OnAuthenticateRequest");

			base.OnAuthenticateRequest(sender, e);

			this.LogDebugIfEnabled("Exiting OnAuthenticateRequest");
		}

		protected override void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("Entering OnPostAuthenticateRequest");

			base.OnPostAuthenticateRequest(sender, e);

			this.LogDebugIfEnabled("Exiting OnPostAuthenticateRequest");
		}

		#endregion
	}
}