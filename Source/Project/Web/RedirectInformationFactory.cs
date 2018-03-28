using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Web;
using RegionOrebroLan.ReportingServices.Extensions;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class RedirectInformationFactory : IRedirectInformationFactory
	{
		#region Fields

		private static Func<string, bool> _isPathWithinAppRootFunction;
		private const string _returnUrlParameterName = "ReturnUrl";

		#endregion

		#region Constructors

		public RedirectInformationFactory(IWebContext webContext)
		{
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
		}

		#endregion

		#region Properties

		protected internal virtual Func<string, bool> IsPathWithinAppRootFunction
		{
			get
			{
				if(_isPathWithinAppRootFunction == null)
				{
					var isPathWithinAppRootMethod = typeof(HttpRuntime).GetMethod("IsPathWithinAppRoot", BindingFlags.NonPublic | BindingFlags.Static);

					// ReSharper disable AssignNullToNotNullAttribute
					_isPathWithinAppRootFunction = (Func<string, bool>) Delegate.CreateDelegate(typeof(Func<string, bool>), isPathWithinAppRootMethod);
					// ReSharper restore AssignNullToNotNullAttribute
				}

				return _isPathWithinAppRootFunction;
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
		protected internal virtual string ReturnUrlParameterName => _returnUrlParameterName;

		protected internal virtual IWebContext WebContext { get; }

		#endregion

		#region Methods

		public virtual IRedirectInformation Create()
		{
			var redirectInformation = new RedirectInformation();

			var returnUrlValue = this.WebContext.HttpRequest.QueryString[this.ReturnUrlParameterName];

			// ReSharper disable InvertIf
			if(!string.IsNullOrEmpty(returnUrlValue))
			{
				if(!Uri.TryCreate(returnUrlValue, UriKind.RelativeOrAbsolute, out var returnUrl))
				{
					redirectInformation.Exception = new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Could not create an url from the value \"{0}\".", returnUrlValue));
				}
				else
				{
					redirectInformation.Url = this.GetRedirectUrl(returnUrl);

					if(redirectInformation.Url.IsAbsoluteUri && !this.IsUrlWithinAppRoot(redirectInformation.Url))
					{
						var redirectUrlValue = redirectInformation.Url.ToStringValue();
						redirectInformation.Exception = new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The url \"{0}\", resolved from return-url \"{1}\", is not a valid redirect-url.", redirectUrlValue, returnUrlValue));
					}
				}
			}
			// ReSharper restore InvertIf

			return redirectInformation;
		}

		protected internal virtual UriBuilder CreateUriBuilder()
		{
			// ReSharper disable AssignNullToNotNullAttribute
			return new UriBuilder(this.WebContext.HttpRequest.Url)
			{
				Fragment = string.Empty,
				Path = string.Empty,
				Query = string.Empty
			};
			// ReSharper restore AssignNullToNotNullAttribute
		}

		protected internal virtual Uri GetRedirectUrl(Uri returnUrl)
		{
			// ReSharper disable All
			if(returnUrl != null && !returnUrl.IsAbsoluteUri)
			{
				var returnUrlBuilder = new UriBuilder(this.CreateUriBuilder().Uri.ToString().TrimEnd('/') + "/" + returnUrl.OriginalString.TrimStart('/'));

				var applicationPath = this.WebContext.HttpRequest.ApplicationPath.TrimEnd('/') + "/";

				if(returnUrlBuilder.Path.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
				{
					var redirectUrlBuilder = this.CreateUriBuilder();

					redirectUrlBuilder.Path = returnUrlBuilder.Path.Substring(applicationPath.Length);

					if(redirectUrlBuilder.Path.TrimEnd('/').Equals("localredirect", StringComparison.OrdinalIgnoreCase))
					{
						var queryString = HttpUtility.ParseQueryString(returnUrlBuilder.Query);

						var urlValue = HttpUtility.UrlDecode(queryString["url"]);

						if(urlValue != null && urlValue.StartsWith("/", StringComparison.OrdinalIgnoreCase))
						{
							if(Uri.TryCreate(urlValue, UriKind.Relative, out var redirectUrl))
								return redirectUrl;
						}
					}
				}
			}
			// ReSharper restore All

			return returnUrl;
		}

		protected internal virtual bool IsUrlWithinAppRoot(Uri url)
		{
			if(url == null)
				throw new ArgumentNullException(nameof(url));

			if(!url.IsAbsoluteUri)
				return this.IsPathWithinAppRootFunction.Invoke(url.OriginalString);

			// ReSharper disable PossibleNullReferenceException
			if(!url.IsLoopback && !string.Equals(this.WebContext.HttpRequest.Url.Host, url.Host, StringComparison.OrdinalIgnoreCase))
				return false;
			// ReSharper restore PossibleNullReferenceException

			return this.IsPathWithinAppRootFunction.Invoke(url.AbsolutePath);
		}

		#endregion
	}
}