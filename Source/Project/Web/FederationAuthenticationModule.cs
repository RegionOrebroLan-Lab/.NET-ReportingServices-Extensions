using System;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Services;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : IHttpModule
	{
		#region Fields

		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public FederationAuthenticationModule() : this(_webContext) { }

		protected internal FederationAuthenticationModule(IWebContext webContext)
		{
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) { }

		public virtual void Init(HttpApplication context)
		{
			var wsFederationAuthenticationModule = FederatedAuthentication.WSFederationAuthenticationModule;

			if(wsFederationAuthenticationModule == null)
				throw new InvalidOperationException("There is no WSFederationAuthenticationModule in the module-pipeline. You must add a WSFederationAuthenticationModule if ReportingServicesFederationAuthenticationModule is in the pipeline.");

			wsFederationAuthenticationModule.RedirectingToIdentityProvider += this.OnRedirectingToIdentityProvider;
			wsFederationAuthenticationModule.SignInError += this.OnSignInError;
			wsFederationAuthenticationModule.SignOutError += this.OnSignOutError;
		}

		protected internal virtual void OnRedirectingToIdentityProvider(object sender, RedirectingToIdentityProviderEventArgs e)
		{
			if(e == null)
				throw new ArgumentNullException(nameof(e));

			var realmUriBuilder = new UriBuilder(e.SignInRequestMessage.Realm);
			var realmQuery = HttpUtility.ParseQueryString(realmUriBuilder.Query);

			// ReSharper disable AssignNullToNotNullAttribute
			var contextUriBuilder = new UriBuilder(this.WebContext.HttpContext.Request.Url);
			// ReSharper restore AssignNullToNotNullAttribute
			var contextQuery = HttpUtility.ParseQueryString(contextUriBuilder.Query);

			foreach(var key in contextQuery.AllKeys)
			{
				realmQuery.Add(key, contextQuery[key]);
			}

			realmUriBuilder.Query = realmQuery.ToString();

			e.SignInRequestMessage.Realm = realmUriBuilder.Uri.ToString();
		}

		protected internal virtual void OnSignInError(object sender, ErrorEventArgs e)
		{
			throw new NotImplementedException("Could not sign in.", e.Exception);
		}

		protected internal virtual void OnSignOutError(object sender, ErrorEventArgs e)
		{
			throw new NotImplementedException("Could not sign out.", e.Exception);
		}

		#endregion
	}
}