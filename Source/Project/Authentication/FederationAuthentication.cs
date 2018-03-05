using System;
using System.Linq;
using System.Security.Principal;
using log4net;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using RegionOrebroLan.ReportingServices.Tracing;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class FederationAuthentication : TraceableComponent, IAuthenticationExtension2
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(FederationAuthentication));
		private static readonly IWebContext _webContext = ServiceLocator.Instance.GetService<IWebContext>();
		private static readonly IWindowsAuthenticationExtension2 _windowsAuthentication = ServiceLocator.Instance.GetService<IWindowsAuthenticationExtension2>();

		#endregion

		#region Constructors

		public FederationAuthentication() : this(_log, _webContext, _windowsAuthentication) { }

		protected internal FederationAuthentication(ILog log, IWebContext webContext, IWindowsAuthenticationExtension2 windowsAuthentication)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
			this.WindowsAuthentication = windowsAuthentication ?? throw new ArgumentNullException(nameof(windowsAuthentication));
		}

		#endregion

		#region Properties

		public virtual string LocalizedName => null;
		protected internal virtual ILog Log { get; }
		protected internal virtual IWebContext WebContext { get; }
		protected internal IWindowsAuthenticationExtension2 WindowsAuthentication { get; }

		#endregion

		#region Methods

		public virtual void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
		{
			userIdentity = this.WebContext.HttpContext?.User?.Identity;

			if(userIdentity == null)
				throw new InvalidOperationException("The http-context-user-identity is null.");

			// It should be IntPtr.Zero otherwhise the RegionOrebroLan.ReportingServices.Authorization.FederationAuthorization.GetPermissions(string userName, IntPtr userToken, SecurityItemType itemType, byte[] secDesc) will fail.
			userId = IntPtr.Zero;
		}

		public virtual void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
		{
			if(requestContext != null && this.Log.IsInfoEnabled)
			{
				this.Log.Info("Request-context-user: " + (requestContext.User?.Name ?? "NULL"));

				if(requestContext.Cookies.Any())
				{
					foreach(var cookie in requestContext.Cookies)
					{
						this.Log.Info("Request-context-cookie: " + cookie.Key + " = " + cookie.Value);
					}
				}
				else
				{
					this.Log.Info("No request-context-cookies.");
				}

				if(requestContext.Headers.Any())
				{
					foreach(var header in requestContext.Headers)
					{
						this.Log.Info("Request-context-header: " + header.Key + " = " + string.Join(", ", header.Value));
					}
				}
				else
				{
					this.Log.Info("No request-context-headers.");
				}
			}

			userIdentity = requestContext?.User;

			if(userIdentity == null)
			{
				//requestContext.Cookies.
			}

			// It should be IntPtr.Zero otherwhise the RegionOrebroLan.ReportingServices.Authorization.FederationAuthorization.GetPermissions(string userName, IntPtr userToken, SecurityItemType itemType, byte[] secDesc) will fail.
			userId = IntPtr.Zero;
		}

		public virtual bool IsValidPrincipalName(string principalName)
		{
			return this.WindowsAuthentication.IsValidPrincipalName(principalName);
		}

		public virtual bool LogonUser(string userName, string password, string authority)
		{
			throw new NotImplementedException();
		}

		public virtual void SetConfiguration(string configuration)
		{
			// Get cookie-name and signing cert location (maybe)
		}

		#endregion
	}
}