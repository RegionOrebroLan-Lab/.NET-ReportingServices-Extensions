using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Configuration;
using System.Web.Security;
using log4net;
using RegionOrebroLan.ReportingServices.Extensions;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : WSFederationAuthenticationModule
	{
		#region Fields

		private static readonly FormsAuthenticationConfiguration _formsAuthenticationConfiguration = new FormsAuthenticationConfiguration();
		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationModule));
		private static readonly IWebContext _webContext = new WebContext();
		private static readonly IRedirectInformationFactory _redirectInformationFactory = new RedirectInformationFactory(_webContext);

		#endregion

		#region Constructors

		public FederationAuthenticationModule() : this(_log, _redirectInformationFactory, _webContext) { }

		public FederationAuthenticationModule(ILog log, IRedirectInformationFactory redirectInformationFactory, IWebContext webContext)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.RedirectInformationFactory = redirectInformationFactory ?? throw new ArgumentNullException(nameof(redirectInformationFactory));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual FormsAuthenticationConfiguration FormsAuthenticationConfiguration => _formsAuthenticationConfiguration;
		protected internal virtual ILog Log { get; }
		protected internal virtual IRedirectInformationFactory RedirectInformationFactory { get; }
		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		protected internal virtual FormsAuthenticationTicket CreateFormsAuthenticationTicket()
		{
			if(!(this.WebContext.HttpContext.User.Identity is WindowsIdentity windowsIdentity))
				throw new InvalidOperationException("The http-context-user-identity must be a windows-identity.");

			var userPrincipalNameClaim = windowsIdentity.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Upn, StringComparison.OrdinalIgnoreCase));

			if(userPrincipalNameClaim == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The http-context-user-identity must have a \"{0}\"-claim.", ClaimTypes.Upn));

			return new FormsAuthenticationTicket(userPrincipalNameClaim.Value, false, this.FormsAuthenticationConfiguration.Timeout.Minutes);
		}

		protected internal virtual void LogDebugIfEnabled(string message, string method)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			this.Log.DebugFormat("FederationAuthenticationModule - {0}: {1}", method, message);
		}

		[SuppressMessage("Microsoft.Naming", "CA1725: Parameter names should match base declaration.")]
		protected override void OnAuthenticateRequest(object sender, EventArgs e)
		{
			base.OnAuthenticateRequest(sender, e);

			this.LogDebugIfEnabled("HttpMethod = " + this.WebContext.HttpRequest.HttpMethod + ", Response-cookies = " + string.Join(", ", this.WebContext.HttpResponse.Cookies.AllKeys) + ", Url = " + this.WebContext.HttpRequest.Url, "OnAuthenticateRequest");

			if(this.WebContext.HttpContext.User != null)
			{
				this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Resolving redirect ({0}).", this.WebContext.HttpRequest.Url), "OnAuthenticateRequest");

				var redirectInformation = this.RedirectInformationFactory.Create();

				if(redirectInformation.Exception != null && this.Log.IsErrorEnabled)
					this.Log.Error(redirectInformation.Exception);

				if(!redirectInformation.Redirect)
					return;

				var formsAuthenticationTicket = this.CreateFormsAuthenticationTicket();

				this.WebContext.HttpResponse.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(formsAuthenticationTicket)));

				var url = redirectInformation.Url.ToStringValue();

				this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Redirecting to {0}, response-cookies = {1}.", url, string.Join(", ", this.WebContext.HttpResponse.Cookies.AllKeys)), "OnAuthenticateRequest");

				this.WebContext.HttpResponse.Redirect(url, false);
				this.WebContext.HttpContext.ApplicationInstance.CompleteRequest();
			}
			else
			{
				this.LogDebugIfEnabled("The http-context-user is null. Redirecting to identity-provider.", "OnAuthenticateRequest");

				this.RedirectToIdentityProvider("passive", this.WebContext.HttpRequest.RawUrl, this.PersistentCookiesOnPassiveRedirects);
			}
		}

		protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
		{
			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Changing reply from \"{0}\" to \"{1}\".", e.SignInRequestMessage.Reply, this.WebContext.HttpRequest.Url), "OnRedirectingToIdentityProvider");

			// ReSharper disable PossibleNullReferenceException
			e.SignInRequestMessage.Reply = this.WebContext.HttpRequest.Url.ToString();
			// ReSharper restore PossibleNullReferenceException

			base.OnRedirectingToIdentityProvider(e);
		}

		#endregion
	}
}