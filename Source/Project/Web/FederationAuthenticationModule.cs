using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IdentityModel.Services;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using log4net;
using RegionOrebroLan.ReportingServices.Extensions;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using RegionOrebroLan.ReportingServices.Security.Principal;
using RegionOrebroLan.ReportingServices.Web.Security;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : WSFederationAuthenticationModule
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthenticationModule));

		#endregion

		#region Constructors

		public FederationAuthenticationModule() : this(ServiceLocator.Instance.GetService<IFormsAuthentication>(), ServiceLocator.Instance.GetService<IFormsAuthenticationTicketFactory>(), _log, ServiceLocator.Instance.GetService<IRedirectInformationFactory>(), ServiceLocator.Instance.GetService<IWebFacade>()) { }

		public FederationAuthenticationModule(IFormsAuthentication formsAuthentication, IFormsAuthenticationTicketFactory formsAuthenticationTicketFactory, ILog log, IRedirectInformationFactory redirectInformationFactory, IWebFacade webFacade)
		{
			this.FormsAuthentication = formsAuthentication ?? throw new ArgumentNullException(nameof(formsAuthentication));
			this.FormsAuthenticationTicketFactory = formsAuthenticationTicketFactory ?? throw new ArgumentNullException(nameof(formsAuthenticationTicketFactory));
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.RedirectInformationFactory = redirectInformationFactory ?? throw new ArgumentNullException(nameof(redirectInformationFactory));
			this.WebFacade = webFacade ?? throw new ArgumentNullException(nameof(webFacade));
		}

		#endregion

		#region Properties

		protected internal virtual IFormsAuthentication FormsAuthentication { get; }
		protected internal virtual IFormsAuthenticationTicketFactory FormsAuthenticationTicketFactory { get; }

		protected internal virtual bool IsInternalServiceRequest
		{
			get
			{
				var request = this.WebFacade.Request;

				// ReSharper disable PossibleNullReferenceException
				var isServiceRequest = string.Equals(".asmx", Path.GetExtension(request.Url.LocalPath), StringComparison.OrdinalIgnoreCase);
				// ReSharper restore PossibleNullReferenceException

				return isServiceRequest && request.Headers.AllKeys.Contains("RSViaWebApp") && request.Headers.AllKeys.Contains("SOAPAction");
			}
		}

		protected internal virtual ILog Log { get; }
		protected internal virtual IRedirectInformationFactory RedirectInformationFactory { get; }
		protected internal virtual IWebFacade WebFacade { get; }

		#endregion

		#region Methods

		protected internal virtual void AuthenticateWithFormsCookie()
		{
			if(!this.WebFacade.Request.Cookies.AllKeys.Contains(this.FormsAuthentication.CookieName))
			{
				var message = string.Format(CultureInfo.InvariantCulture, "The cookie \"{0}\" is not present.", this.FormsAuthentication.CookieName);

				this.LogDebugIfEnabled(message, "OnAuthenticateRequest");

				var exception = new InvalidOperationException(message);

				if(this.Log.IsErrorEnabled)
					this.Log.Error(exception);

				throw exception;
			}

			var cookie = this.WebFacade.Request.Cookies[this.FormsAuthentication.CookieName];

			// ReSharper disable PossibleNullReferenceException
			var ticket = this.FormsAuthentication.Decrypt(cookie.Value);
			// ReSharper restore PossibleNullReferenceException

			if(ticket.Expired)
			{
				this.LogDebugIfEnabled("The ticket has expired.", "AuthenticateWithFormsCookie");

				return;
			}

			var windowsIdentity = new WindowsIdentity(ticket.Name);

			this.WebFacade.Context.User = new WindowsFederationPrincipal(new WindowsFederationIdentity(new[]
			{
				new Claim(ClaimTypes.Name, windowsIdentity.Name),
				new Claim(ClaimTypes.Upn, ticket.Name)
			}));
		}

		protected internal virtual IFormsAuthenticationTicket CreateFormsAuthenticationTicket()
		{
			if(!(this.WebFacade.User.Identity is IWindowsFederationIdentity windowsFederationIdentity))
				throw new InvalidOperationException("The http-context-user-identity must be a windows-federation-identity.");

			var userPrincipalName = windowsFederationIdentity.UserPrincipalName;

			if(string.IsNullOrEmpty(userPrincipalName))
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The http-context-user-identity must have a \"{0}\"-claim.", ClaimTypes.Upn));

			return this.FormsAuthenticationTicketFactory.Create(userPrincipalName);
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
			if(this.RedirectIfTrailingSlashIsMissing())
				return;

			base.OnAuthenticateRequest(sender, e);

			//this.LogDebugIfEnabled("HttpMethod = " + this.WebFacade.Request.HttpMethod + ", Request-cookies = " + string.Join(", ", this.WebFacade.Request.Cookies.AllKeys) + ", Response-cookies = " + string.Join(", ", this.WebFacade.Response.Cookies.AllKeys) + ", Url = " + this.WebFacade.Request.Url, "OnAuthenticateRequest");

			if(this.WebFacade.User != null)
			{
				this.RedirectUserIfNecessary();
			}
			else
			{
				if(this.IsInternalServiceRequest)
				{
					this.LogDebugIfEnabled("The http-context-user is null and the request is an internal service-request. Authenticating with forms-cookie.", "OnAuthenticateRequest");

					this.AuthenticateWithFormsCookie();

					return;
				}

				this.LogDebugIfEnabled("The http-context-user is null. Redirecting to identity-provider.", "OnAuthenticateRequest");

				this.RedirectToIdentityProvider("passive", this.WebFacade.Request.RawUrl, this.PersistentCookiesOnPassiveRedirects);
			}
		}

		protected override void OnRedirectingToIdentityProvider(RedirectingToIdentityProviderEventArgs e)
		{
			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Changing reply from \"{0}\" to \"{1}\".", e.SignInRequestMessage.Reply, this.WebFacade.Request.Url), "OnRedirectingToIdentityProvider");

			// ReSharper disable PossibleNullReferenceException
			e.SignInRequestMessage.Reply = this.WebFacade.Request.Url.ToString();
			// ReSharper restore PossibleNullReferenceException

			base.OnRedirectingToIdentityProvider(e);
		}

		protected internal virtual bool RedirectIfTrailingSlashIsMissing()
		{
			var redirectInformation = new RedirectInformation();

			var url = this.WebFacade.Request.Url;

			if(url != null && !url.LocalPath.EndsWith("/", StringComparison.OrdinalIgnoreCase) && string.IsNullOrEmpty(Path.GetExtension(url.LocalPath)))
			{
				var uriBuilder = new UriBuilder(url);

				uriBuilder.Path += "/";

				redirectInformation.Url = uriBuilder.Uri;
			}

			if(!redirectInformation.Redirect)
				return false;

			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "The path must end with a slash. Redirecting from {0} to {1}.", this.WebFacade.Request.Url, redirectInformation.Url), "RedirectIfTrailingSlashMissing");

			this.WebFacade.Response.Redirect(redirectInformation.Url.ToStringValue(), false);
			this.WebFacade.Context.ApplicationInstance.CompleteRequest();

			return true;
		}

		protected internal virtual void RedirectUserIfNecessary()
		{
			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Resolving redirect ({0}).", this.WebFacade.Request.Url), "RedirectUserIfNecessary");

			var redirectInformation = this.RedirectInformationFactory.Create();

			if(redirectInformation.Exception != null && this.Log.IsErrorEnabled)
				this.Log.Error(redirectInformation.Exception);

			if(!redirectInformation.Redirect)
				return;

			var formsAuthenticationTicket = this.CreateFormsAuthenticationTicket();

			this.WebFacade.Response.Cookies.Add(new HttpCookie(this.FormsAuthentication.CookieName, this.FormsAuthentication.Encrypt(formsAuthenticationTicket)));

			foreach(var key in this.WebFacade.Request.Cookies.AllKeys)
			{
				if(key == null)
					continue;

				if(!key.StartsWith(this.FederationConfiguration.CookieHandler.Name, StringComparison.OrdinalIgnoreCase))
					continue;

				if(this.WebFacade.Response.Cookies.AllKeys.Contains(key))
					continue;

				// ReSharper disable AssignNullToNotNullAttribute
				this.WebFacade.Response.Cookies.Add(this.WebFacade.Request.Cookies[key]);
				// ReSharper restore AssignNullToNotNullAttribute
			}

			var url = redirectInformation.Url.ToStringValue();

			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Redirecting to {0}, response-cookies = {1}.", url, string.Join(", ", this.WebFacade.Response.Cookies.AllKeys)), "RedirectUserIfNecessary");

			this.WebFacade.Response.Redirect(url, false);
			this.WebFacade.Context.ApplicationInstance.CompleteRequest();
		}

		#endregion
	}
}