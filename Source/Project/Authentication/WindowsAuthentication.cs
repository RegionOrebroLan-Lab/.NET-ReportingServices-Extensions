﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Tracing;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class WindowsAuthentication : TraceableComponent, IWindowsAuthenticationExtension2
	{
		#region Fields

		private static readonly IWebContext _webContext = new WebContext();

		#endregion

		#region Constructors

		public WindowsAuthentication() : this(_webContext) { }

		protected internal WindowsAuthentication(IWebContext webContext)
		{
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
			this.WindowsAuthenticationInternal = new Microsoft.ReportingServices.Authentication.WindowsAuthentication();
		}

		#endregion

		#region Properties

		public virtual string LocalizedName => this.WindowsAuthenticationInternal.LocalizedName;
		protected internal virtual IWebContext WebContext { get; }
		protected internal Microsoft.ReportingServices.Authentication.WindowsAuthentication WindowsAuthenticationInternal { get; }

		#endregion

		#region Methods

		public virtual void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
		{
			this.GetUserInfo(null, out userIdentity, out userId);
		}

		public virtual void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
		{
			if(requestContext != null && this.TraceLog != null)
				this.WriteTrace(string.Format(CultureInfo.InvariantCulture, "The request-context-identity is \"{0}\".", requestContext.User.Name), TraceLevel.Verbose);

			var httpContext = this.WebContext.HttpContext;

			if(httpContext != null)
			{
				userIdentity = httpContext.User.Identity;
			}
			else
			{
				userIdentity = null;

				var exceptionMessage = "The The http-context is null.";

				if(requestContext != null)
					exceptionMessage += string.Format(CultureInfo.InvariantCulture, " The request-context-identity is \"{0}\".", requestContext.User != null ? requestContext.User.Name : "NULL");

				this.HandleException(new InvalidOperationException(exceptionMessage));
			}

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

		public virtual byte[] PrincipalNameToSid(string name)
		{
			return this.WindowsAuthenticationInternal.PrincipalNameToSid(name);
		}

		public virtual void SetConfiguration(string configuration) { }

		public virtual string SidToPrincipalName(byte[] sid)
		{
			return this.WindowsAuthenticationInternal.SidToPrincipalName(sid);
		}

		#endregion
	}
}