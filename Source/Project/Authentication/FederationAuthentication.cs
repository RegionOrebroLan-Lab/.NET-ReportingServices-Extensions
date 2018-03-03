using System;
using System.Security.Principal;
using Microsoft.ReportingServices.Authentication;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Tracing;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class FederationAuthentication : TraceableComponent, IAuthenticationExtension2
	{
		#region Fields

		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public FederationAuthentication() : this(_webContext) { }

		protected internal FederationAuthentication(IWebContext webContext)
		{
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
			this.WindowsAuthentication = new WindowsAuthentication();
		}

		#endregion

		#region Properties

		public virtual string LocalizedName => null;
		protected internal virtual IWebContext WebContext { get; }
		protected internal WindowsAuthentication WindowsAuthentication { get; }

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