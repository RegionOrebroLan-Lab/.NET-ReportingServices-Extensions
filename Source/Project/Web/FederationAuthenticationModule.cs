using System;
using System.IdentityModel.Services;
using System.Linq;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class FederationAuthenticationModule : BasicHttpModule
	{
		#region Properties

		protected internal virtual IHttpApplication Application { get; set; }

		#endregion

		#region Methods

		public override void Initialize(IHttpApplication application)
		{
			this.Application = application ?? throw new ArgumentNullException(nameof(application));

			var wsFederationAuthenticationModule = (WSFederationAuthenticationModule) application.Modules.Select(module => module.Value).FirstOrDefault(module => module is WSFederationAuthenticationModule);

			if(wsFederationAuthenticationModule == null)
				throw new InvalidOperationException("There is no WSFederationAuthenticationModule in the module-pipeline. You must add a WSFederationAuthenticationModule if FederationAuthenticationModule is in the pipeline.");

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
			var contextUriBuilder = new UriBuilder(this.Application.Context.Request.Url);
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
			throw new InvalidOperationException("Could not sign in.", e.Exception);
		}

		protected internal virtual void OnSignOutError(object sender, ErrorEventArgs e)
		{
			throw new InvalidOperationException("Could not sign out.", e.Exception);
		}

		#endregion
	}
}