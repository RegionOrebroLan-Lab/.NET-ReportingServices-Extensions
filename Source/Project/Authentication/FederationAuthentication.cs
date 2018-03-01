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
			this.WindowsAuthenticationInternal = new WindowsAuthentication();
		}

		#endregion

		#region Properties

		public virtual string LocalizedName => null;
		protected internal virtual IWebContext WebContext { get; }
		protected internal WindowsAuthentication WindowsAuthenticationInternal { get; }

		#endregion

		#region Methods

		public virtual void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
		{
			userIdentity = this.WebContext.HttpContext?.User?.Identity;

			if(userIdentity == null)
				throw new InvalidOperationException("The http-context-user-identity is null.");

			var windowsIdentity = userIdentity as WindowsIdentity;
			userId = windowsIdentity?.Token ?? IntPtr.Zero;
		}

		public virtual void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
		{
			userIdentity = requestContext?.User;
			var windowsIdentity = userIdentity as WindowsIdentity;
			userId = windowsIdentity?.Token ?? IntPtr.Zero;
		}

		public virtual bool IsValidPrincipalName(string principalName)
		{
			return this.WindowsAuthenticationInternal.IsValidPrincipalName(principalName);
		}

		public virtual bool LogonUser(string userName, string password, string authority)
		{
			throw new NotImplementedException();
		}

		public virtual void SetConfiguration(string configuration) { }

		#endregion
	}
}