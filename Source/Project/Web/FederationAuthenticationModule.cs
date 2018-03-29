using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Claims;
using System.IdentityModel.Services;
using System.Linq;
using System.Security.Principal;
using System.Web;
using log4net;
using RegionOrebroLan.ReportingServices.Extensions;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using RegionOrebroLan.ReportingServices.Web.Security;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : WSFederationAuthenticationModule
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationModule));

		#endregion

		#region Constructors

		public FederationAuthenticationModule() : this(ServiceLocator.Instance.GetService<IFormsAuthentication>(), ServiceLocator.Instance.GetService<IFormsAuthenticationTicketFactory>(), _log, ServiceLocator.Instance.GetService<IRedirectInformationFactory>(), ServiceLocator.Instance.GetService<IWebContext>()) { }

		public FederationAuthenticationModule(IFormsAuthentication formsAuthentication, IFormsAuthenticationTicketFactory formsAuthenticationTicketFactory, ILog log, IRedirectInformationFactory redirectInformationFactory, IWebContext webContext)
		{
			this.FormsAuthentication = formsAuthentication ?? throw new ArgumentNullException(nameof(formsAuthentication));
			this.FormsAuthenticationTicketFactory = formsAuthenticationTicketFactory ?? throw new ArgumentNullException(nameof(formsAuthenticationTicketFactory));
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.RedirectInformationFactory = redirectInformationFactory ?? throw new ArgumentNullException(nameof(redirectInformationFactory));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual IFormsAuthentication FormsAuthentication { get; }
		protected internal virtual IFormsAuthenticationTicketFactory FormsAuthenticationTicketFactory { get; }
		protected internal virtual ILog Log { get; }
		protected internal virtual IRedirectInformationFactory RedirectInformationFactory { get; }
		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		protected internal virtual IFormsAuthenticationTicket CreateFormsAuthenticationTicket()
		{
			if(!(this.WebContext.HttpContext.User.Identity is WindowsIdentity windowsIdentity))
				throw new InvalidOperationException("The http-context-user-identity must be a windows-identity.");

			var userPrincipalNameClaim = windowsIdentity.Claims.FirstOrDefault(claim => claim.Type.Equals(ClaimTypes.Upn, StringComparison.OrdinalIgnoreCase));

			if(userPrincipalNameClaim == null)
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The http-context-user-identity must have a \"{0}\"-claim.", ClaimTypes.Upn));

			return this.FormsAuthenticationTicketFactory.Create(userPrincipalNameClaim.Value);
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

				this.WebContext.HttpResponse.Cookies.Add(new HttpCookie(this.FormsAuthentication.CookieName, this.FormsAuthentication.Encrypt(formsAuthenticationTicket)));

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