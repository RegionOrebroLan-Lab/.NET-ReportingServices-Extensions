using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class IdentityResolver : SessionAuthenticationModule, IIdentityResolver
	{
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
			var httpContext = this.CreateHttpContext();

			foreach(var cookie in cookies)
			{
				httpContext.Request.Cookies.Add(new HttpCookie(cookie.Key, cookie.Value));
			}

			var cookieBytes = this.CookieHandler.Read(this.CookieHandler.Name, httpContext);

			var sessionSecurityToken = this.ReadSessionTokenFromCookie(cookieBytes);

			var claimsIdentities = this.ValidateSessionToken(sessionSecurityToken);

			if(claimsIdentities.Count > 1)
				throw new InvalidOperationException("There are multiple claims-identities.");

			return claimsIdentities.FirstOrDefault();
		}

		#endregion
	}
}