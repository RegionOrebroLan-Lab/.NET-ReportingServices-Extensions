using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using log4net;
using Microsoft.IdentityModel.WindowsTokenService;
using RegionOrebroLan.ReportingServices.Web.Security;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class IdentityResolver : SessionAuthenticationModule, IIdentityResolver
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(IdentityResolver));

		#endregion

		#region Constructors

		public IdentityResolver(IFormsAuthentication formsAuthentication) : this(formsAuthentication, _log) { }

		protected internal IdentityResolver(IFormsAuthentication formsAuthentication, ILog log)
		{
			this.FormsAuthentication = formsAuthentication ?? throw new ArgumentNullException(nameof(formsAuthentication));
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
		}

		#endregion

		#region Properties

		protected internal virtual IFormsAuthentication FormsAuthentication { get; }
		protected internal virtual ILog Log { get; }

		#endregion

		#region Methods

		protected internal virtual HttpContext CreateHttpContext()
		{
			return new HttpContext(this.CreateHttpRequest(), this.CreateHttpResponse());
		}

		protected internal virtual HttpRequest CreateHttpRequest()
		{
			return new HttpRequest(null, "http://localhost/", null);
		}

		protected internal virtual HttpResponse CreateHttpResponse()
		{
			return new HttpResponse(null);
		}

		public virtual IIdentity GetIdentity(IDictionary<string, string> cookies)
		{
			cookies = cookies ?? new Dictionary<string, string>();

			if(!cookies.TryGetValue(this.FormsAuthentication.CookieName, out var cookieValue))
			{
				this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "There are no relevant cookies. Available cookies: {0}.", string.Join(", ", cookies.Keys)), "GetIdentity");

				return null;
			}

			try
			{
				var ticket = this.FormsAuthentication.Decrypt(cookieValue);

				if(ticket == null)
				{
					this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "The ticket from cookie \"{0}\" is null.", this.FormsAuthentication.CookieName), "GetIdentity");

					return null;
				}

				if(ticket.Expired)
				{
					this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "The ticket from cookie \"{0}\" has expired.", this.FormsAuthentication.CookieName), "GetIdentity");

					return null;
				}

				var windowsIdentity = S4UClient.UpnLogon(ticket.Name);

				windowsIdentity = new WindowsIdentity(windowsIdentity.Token, "Federation", WindowsAccountType.Normal, true);

				windowsIdentity.AddClaim(new Claim(ClaimTypes.Upn, ticket.Name));

				return windowsIdentity;
			}
			catch(Exception exception)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "Could not get identity from cookies. Available cookies: {0}.", string.Join(", ", cookies.Keys));
				const string method = "GetIdentity";

				this.LogErrorIfEnabled(exception, message, method);

				throw new InvalidOperationException("IdentityResolver - " + method + ": " + message, exception);
			}
		}

		protected internal virtual void LogDebugIfEnabled(string message, string method)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			this.Log.DebugFormat("IdentityResolver - {0}: {1}", method, message);
		}

		protected internal virtual void LogErrorIfEnabled(string message, string method)
		{
			this.LogErrorIfEnabled(null, message, method);
		}

		protected internal virtual void LogErrorIfEnabled(Exception exception, string method)
		{
			this.LogErrorIfEnabled(exception, null, method);
		}

		protected internal virtual void LogErrorIfEnabled(Exception exception, string message, string method)
		{
			if(!this.Log.IsErrorEnabled)
				return;

			var prefix = string.Format(CultureInfo.InvariantCulture, "IdentityResolver - {0}: ", method);

			message = prefix + message;

			if(exception == null)
				this.Log.Error(message);
			else
				this.Log.Error(message, exception);
		}

		#endregion
	}
}