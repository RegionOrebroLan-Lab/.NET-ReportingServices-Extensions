using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using log4net;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Web;
using ReportingServicesWindowsAuthentication = Microsoft.ReportingServices.Authentication.WindowsAuthentication;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class WindowsAuthentication : IWindowsAuthenticationExtension2
	{
		#region Fields

		private static readonly IIdentityResolverFactory _identityResolverFactory = new IdentityResolverFactory();
		private static readonly ILog _log = LogManager.GetLogger(typeof(WindowsAuthentication));
		private static readonly IWebContext _webContext = new WebContext();
		private static readonly IWindowsAuthenticationExtension2 _windowsAuthenticationInternal = new ReportingServicesWindowsAuthentication();

		#endregion

		#region Constructors

		public WindowsAuthentication() : this(_identityResolverFactory, _log, _webContext, _windowsAuthenticationInternal) { }

		public WindowsAuthentication(IIdentityResolverFactory identityResolverFactory, ILog log, IWebContext webContext, IWindowsAuthenticationExtension2 windowsAuthenticationInternal)
		{
			this.IdentityResolverFactory = identityResolverFactory ?? throw new ArgumentNullException(nameof(identityResolverFactory));
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebContext = webContext ?? throw new ArgumentNullException(nameof(webContext));
			this.WindowsAuthenticationInternal = windowsAuthenticationInternal ?? throw new ArgumentNullException(nameof(windowsAuthenticationInternal));
		}

		#endregion

		#region Properties

		protected internal virtual IIdentityResolver IdentityResolver { get; set; }
		protected internal virtual IIdentityResolverFactory IdentityResolverFactory { get; }
		public virtual string LocalizedName => null;
		protected internal virtual ILog Log { get; }
		protected internal virtual IWebContext WebContext { get; }
		protected internal IWindowsAuthenticationExtension2 WindowsAuthenticationInternal { get; }

		#endregion

		#region Methods

		protected internal virtual string GetIdentityName(IIdentity identity)
		{
			if(identity == null)
				return "NULL";

			if(!identity.IsAuthenticated)
				return "Anonymous";

			return identity.Name;
		}

		public virtual void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
		{
			userId = IntPtr.Zero;
			userIdentity = this.WebContext.HttpContext?.User?.Identity;

			if(userIdentity == null || !userIdentity.IsAuthenticated)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "The http-context-user-identity can not be \"{0}\".", this.GetIdentityName(userIdentity));

				if(this.WebContext.HttpContext == null)
					message += " The http-context is null.";
				else if(this.WebContext.HttpContext.User == null)
					message += " The http-context-user is null.";

				const string method = "GetUserInfo (2 parameters)";

				this.LogErrorIfEnabled(message, method);

				throw new InvalidOperationException("WindowsAuthentication - " + method + ": " + message);
			}

			if(!(userIdentity is WindowsIdentity windowsIdentity))
				this.ThrowIdentityMustBeAWindowsIdentityException("GetUserInfo (2 parameters)", "http-context-user-identity");
			else
				userId = windowsIdentity.Token;
		}

		public virtual void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
		{
			userId = IntPtr.Zero;
			userIdentity = requestContext.User;

			var cookies = requestContext.Cookies ?? new Dictionary<string, string>();

			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "User-identity = {0}, cookies = {1}.", userIdentity == null ? "NULL" : (userIdentity.IsAuthenticated ? userIdentity.Name : "Anonymous"), string.Join(", ", cookies.Keys)), "GetUserInfo (3 parameters)");

			if(userIdentity == null)
			{
				try
				{
					userIdentity = this.IdentityResolver.GetIdentity(cookies);
				}
				catch(Exception exception)
				{
					const string message = "Could not get identity from cookies.";
					const string method = "GetUserInfo (3 parameters)";

					this.LogErrorIfEnabled(exception, message, method);

					throw new InvalidOperationException("WindowsAuthentication - " + method + ": " + message, exception);
				}
			}

			if(userIdentity == null)
				return;

			if(!(userIdentity is WindowsIdentity windowsIdentity))
				this.ThrowIdentityMustBeAWindowsIdentityException("GetUserInfo (3 parameters)", "request-context-user");
			else
				userId = windowsIdentity.Token;
		}

		public virtual bool IsValidPrincipalName(string principalName)
		{
			return this.WindowsAuthenticationInternal.IsValidPrincipalName(principalName);
		}

		protected internal virtual void LogDebugIfEnabled(string message, string method)
		{
			if(!this.Log.IsDebugEnabled)
				return;

			this.Log.DebugFormat("WindowsAuthentication - {0}: {1}", method, message);
		}

		protected internal virtual void LogErrorIfEnabled(string message, string method)
		{
			this.LogErrorIfEnabled(null, message, method);
		}

		protected internal virtual void LogErrorIfEnabled(Exception exception, string method)
		{
			this.LogErrorIfEnabled(exception, null, method);
		}

		protected internal virtual void LogErrorIfEnabled(Exception exception, string message, string method)
		{
			if(!this.Log.IsErrorEnabled)
				return;

			var prefix = string.Format(CultureInfo.InvariantCulture, "WindowsAuthentication - {0}: ", method);

			message = prefix + message;

			if(exception == null)
				this.Log.Error(message);
			else
				this.Log.Error(message, exception);
		}

		public virtual bool LogonUser(string userName, string password, string authority)
		{
			this.LogErrorIfEnabled("The method is not implemented.", "LogonUser");

			throw new NotImplementedException();
		}

		public virtual byte[] PrincipalNameToSid(string name)
		{
			return this.WindowsAuthenticationInternal.PrincipalNameToSid(name);
		}

		public virtual void SetConfiguration(string configuration)
		{
			this.LogDebugIfEnabled(string.Format(CultureInfo.InvariantCulture, "Configuration = \"{0}\".", configuration), "SetConfiguration");

			try
			{
				this.IdentityResolver = this.IdentityResolverFactory.Create(configuration);
			}
			catch(Exception exception)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "Could not create an identity-resolver from configuraton \"{0}\".", configuration);

				this.LogErrorIfEnabled(exception, message, "SetConfiguration");

				throw new InvalidOperationException(message, exception);
			}
		}

		public virtual string SidToPrincipalName(byte[] sid)
		{
			return this.WindowsAuthenticationInternal.SidToPrincipalName(sid);
		}

		protected internal virtual void ThrowIdentityMustBeAWindowsIdentityException(string method, string parameterName)
		{
			var message = string.Format(CultureInfo.InvariantCulture, "The {0} must be a windows-identity.", parameterName);

			this.LogErrorIfEnabled(message, method);

			throw new InvalidOperationException("WindowsAuthentication - " + method + ": " + message);
		}

		#endregion
	}
}