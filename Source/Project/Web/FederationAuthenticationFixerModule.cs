using System;
using System.Globalization;
using System.Reflection;
using System.Web;
using log4net;
using Microsoft.ReportingServices.Interfaces;
using Microsoft.ReportingServices.Library;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationFixerModule : IHttpModule
	{
		#region Fields

		private const AuthenticationType _defaultAuthenticationType = AuthenticationType.Windows;
		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationFixerModule));
		private static Type _webConfigUtilType;
		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public FederationAuthenticationFixerModule() : this(_log, _webContext) { }

		public FederationAuthenticationFixerModule(ILog log, IWebContext webContext)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual AuthenticationType AuthenticationType
		{
			// ReSharper disable PossibleNullReferenceException
			get => (AuthenticationType) this.WebConfigUtilType.GetField("m_authMode", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
			set => this.WebConfigUtilType.GetField("m_authMode", BindingFlags.NonPublic | BindingFlags.Static).SetValue(null, value);
			// ReSharper restore PossibleNullReferenceException
		}

		protected internal virtual AuthenticationType DefaultaAuthenticationType => _defaultAuthenticationType;
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

		public virtual void Dispose() { }

		public virtual void Init(HttpApplication context)
		{
			context.AuthenticateRequest += this.OnAuthenticateRequest;
			context.EndRequest += this.OnEndRequest;
			context.PostAuthenticateRequest += this.OnPostAuthenticateRequest;
		}

		protected internal virtual void LogDebugIfEnabled(string message)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			var user = this.WebContext.HttpContext.User;
			var userName = user == null ? "NULL" : (user.Identity.IsAuthenticated ? user.Identity.Name : "Anonymous");

			this.Log.Debug(string.Format(CultureInfo.InvariantCulture, "FederationAuthenticationFixerModule - {0}: user = \"{1}\", url = {2}", message, userName, this.WebContext.HttpRequest.Url));
		}

		protected internal virtual void OnAuthenticateRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("OnAuthenticateRequest");

			var authenticationType = this.AuthenticationType;

			if(authenticationType == AuthenticationType.Forms)
				return;

			this.LogDebugIfEnabled("OnAuthenticateRequest - changing authentication-type to \"Forms\"");

			this.AuthenticationType = AuthenticationType.Forms;
		}

		protected internal virtual void OnEndRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("OnEndRequest");

			var authenticationType = this.AuthenticationType;

			if(authenticationType == this.DefaultaAuthenticationType)
				return;

			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "OnEndRequest - changing authentication-type to \"{0}\"", this.DefaultaAuthenticationType));

			this.AuthenticationType = this.DefaultaAuthenticationType;
		}

		protected internal virtual void OnPostAuthenticateRequest(object sender, EventArgs e)
		{
			this.LogDebugIfEnabled("OnPostAuthenticateRequest");

			var authenticationType = this.AuthenticationType;

			if(authenticationType == this.DefaultaAuthenticationType)
				return;

			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "OnPostAuthenticateRequest - changing authentication-type to \"{0}\"", this.DefaultaAuthenticationType));

			this.AuthenticationType = this.DefaultaAuthenticationType;
		}

		#endregion
	}
}