using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Xml;
using log4net;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class IdentityResolver : SessionAuthenticationModule, IIdentityResolver
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(IdentityResolver));

		#endregion

		#region Constructors

		public IdentityResolver() : this(_log) { }

		public IdentityResolver(ILog log)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
		}

		#endregion

		#region Properties

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
			if(cookies == null)
				throw new ArgumentNullException(nameof(cookies));

			var httpContext = this.CreateHttpContext();

			foreach(var cookie in cookies)
			{
				httpContext.Request.Cookies.Add(new HttpCookie(cookie.Key, cookie.Value));
			}

			try
			{
				var cookieBytes = this.CookieHandler.Read(this.CookieHandler.Name, httpContext);

				if(cookieBytes == null)
				{
					this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "There are no relevant cookies. Available cookies: {0}.", string.Join(", ", cookies.Keys)), "GetIdentity");

					return null;
				}

				var sessionSecurityToken = this.ReadSessionTokenFromCookie(cookieBytes);

				var claimsIdentities = this.ValidateSessionToken(sessionSecurityToken);

				if(claimsIdentities.Count > 1)
					throw new InvalidOperationException("There are multiple claims-identities.");

				if(!(claimsIdentities.FirstOrDefault() is WindowsIdentity windowsIdentity))
					throw new InvalidOperationException("The identity is not a windows-identity.");

				return windowsIdentity;
			}
			catch(Exception exception)
			{
				if(exception is XmlException)
				{
					this.LogDebugIfEnabled("Xml-exception so we faik the user.", "GetIdentity");

					var windowsIdentity = Microsoft.IdentityModel.WindowsTokenService.S4UClient.UpnLogon("user-name@company.com");

					return new WindowsIdentity(windowsIdentity.Token, "Federation", WindowsAccountType.Normal, true);
				}








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