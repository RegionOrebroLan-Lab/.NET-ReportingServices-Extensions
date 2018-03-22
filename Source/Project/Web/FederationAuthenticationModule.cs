using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Services;
using System.Reflection;
using log4net;
using Microsoft.ReportingServices.Library;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : WSFederationAuthenticationModule
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationModule));
		private static Type _webConfigUtilType;
		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public FederationAuthenticationModule() : this(_log, _webContext) { }

		public FederationAuthenticationModule(ILog log, IWebContext webContext)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual ILog Log { get; }

		protected internal virtual Type WebConfigUtilType
		{
			get
			{
				// ReSharper disable InvertIf
				if(_webConfigUtilType == null)
				{
					// ReSharper disable PossibleNullReferenceException
					var assemblyQualifiedName = typeof(TraceEvent).AssemblyQualifiedName.Replace(".TraceEvent", ".WebConfigUtil");
					// ReSharper restore PossibleNullReferenceException

					_webConfigUtilType = Type.GetType(assemblyQualifiedName, true);
				}
				// ReSharper restore InvertIf

				return _webConfigUtilType;
			}
		}

		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		protected internal virtual void LogDebugIfEnabled(string method)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			var user = this.WebContext.HttpContext.User;
			var userName = user == null ? "NULL" : (user.Identity.IsAuthenticated ? user.Identity.Name : "Anonymous");

			this.Log.Debug(string.Format(CultureInfo.InvariantCulture, "FederationAuthenticationModule - {0}: user = \"{1}\", url = {2}", method, userName, this.WebContext.HttpRequest.Url));
		}

		[SuppressMessage("Microsoft.Naming", "CA1725: Parameter names should match base declaration")]
		protected override void OnAuthenticateRequest(object sender, EventArgs e)
		{




			// När vi kommer hit behöver vi manipulera AuthenticationType















			// ReSharper disable PossibleNullReferenceException
			this.WebConfigUtilType.GetField("m_authMode", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, Microsoft.ReportingServices.Interfaces.AuthenticationType.Forms);
			// ReSharper restore PossibleNullReferenceException

			this.LogDebugIfEnabled("Entering OnAuthenticateRequest");

			base.OnAuthenticateRequest(sender, e);

			this.LogDebugIfEnabled("Exiting OnAuthenticateRequest");
		}

		[SuppressMessage("Microsoft.Naming", "CA1725: Parameter names should match base declaration")]
		protected override void OnEndRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("Entering OnEndRequest");

			base.OnEndRequest(sender, e);

			this.LogDebugIfEnabled("Exiting OnEndRequest");
		}

		protected override void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			// ReSharper disable PossibleNullReferenceException
			this.WebConfigUtilType.GetField("m_authMode", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, Microsoft.ReportingServices.Interfaces.AuthenticationType.Windows);
			// ReSharper restore PossibleNullReferenceException

			this.LogDebugIfEnabled("Entering OnPostAuthenticateRequest");

			base.OnPostAuthenticateRequest(sender, e);

			this.LogDebugIfEnabled("Exiting OnPostAuthenticateRequest");
		}

		#endregion
	}
}