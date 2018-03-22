using System;
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

		public virtual void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
		{
			this.LogDebugIfEnabled("Entering GetUserInfo with 2 parameters.");

			userIdentity = this.WebContext.HttpContext?.User?.Identity;

			if(userIdentity == null || !userIdentity.IsAuthenticated)
			{
				var userIdentityValue = userIdentity == null ? "null." : "anonymous";
				var message = string.Format(CultureInfo.InvariantCulture, "GetUserInfo with 2 parameters: The http-context-user-identity can not be {0}.", userIdentityValue);

				if(this.WebContext.HttpContext == null)
					message += " The http-context is null.";
				else if(this.WebContext.HttpContext.User == null)
					message += " The http-context-user is null.";

				this.LogErrorIfEnabled(message);

				throw new InvalidOperationException(message);
			}

			if(!(userIdentity is WindowsIdentity windowsIdentity))
			{
				const string message = "GetUserInfo with 2 parameters: The http-context-user-identity must be a windows-identity.";

				this.LogErrorIfEnabled(message);

				throw new InvalidOperationException(message);
			}

			userId = windowsIdentity.Token;

			this.LogDebugIfEnabled("Exiting GetUserInfo with 2 parameters.");
		}

		public virtual void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
		{
			this.LogDebugIfEnabled("Entering GetUserInfo with 3 parameters.");

			userIdentity = requestContext?.User ?? this.IdentityResolver.GetIdentity(requestContext?.Cookies);

			if(userIdentity == null || !userIdentity.IsAuthenticated)
			{
				var userIdentityValue = userIdentity == null ? "null." : "anonymous";
				var message = string.Format(CultureInfo.InvariantCulture, "GetUserInfo with 3 parameters: The user-identity could not be resolved. The user-identity can not be {0}.", userIdentityValue);

				if(requestContext == null)
				{
					message += " The request-context is null.";
				}
				else if(requestContext.User == null && requestContext.Cookies == null)
				{
					message += " The request-context-cookies is null.";
				}

				this.LogErrorIfEnabled(message);

				throw new InvalidOperationException(message);
			}

			if(!(userIdentity is WindowsIdentity windowsIdentity))
			{
				const string message = "GetUserInfo with 3 parameters: The user-identity must be a windows-identity.";

				this.LogErrorIfEnabled(message);

				throw new InvalidOperationException(message);
			}

			userId = windowsIdentity.Token;

			this.LogDebugIfEnabled("Exiting GetUserInfo with 3 parameters.");
		}

		public virtual bool IsValidPrincipalName(string principalName)
		{
			return this.WindowsAuthenticationInternal.IsValidPrincipalName(principalName);
		}

		protected internal virtual void LogDebugIfEnabled(string message)
		{
			if(this.Log.IsDebugEnabled)
				this.Log.Debug(message);
		}

		protected internal virtual void LogErrorIfEnabled(string message)
		{
			this.LogErrorIfEnabled(message, null);
		}

		protected internal virtual void LogErrorIfEnabled(Exception exception)
		{
			this.LogErrorIfEnabled(null, exception);
		}

		protected internal virtual void LogErrorIfEnabled(string message, Exception exception)
		{
			if(!this.Log.IsErrorEnabled)
				return;

			if(message != null)
			{
				if(exception != null)
					this.Log.Error(message, exception);
				else
					this.Log.Error(message);
			}
			else
			{
				this.Log.Error(exception);
			}
		}

		public virtual bool LogonUser(string userName, string password, string authority)
		{
			if(this.Log.IsDebugEnabled)
				this.Log.Debug("LogonUser");

			throw new NotImplementedException();
		}

		public virtual byte[] PrincipalNameToSid(string name)
		{
			return this.WindowsAuthenticationInternal.PrincipalNameToSid(name);
		}

		public virtual void SetConfiguration(string configuration)
		{
			try
			{
				this.IdentityResolver = this.IdentityResolverFactory.Create(configuration);
			}
			catch(Exception exception)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "Could not create an identity-resolver from configuraton \"{0}\".", configuration);

				this.LogErrorIfEnabled(message, exception);

				throw new InvalidOperationException(message, exception);
			}
		}

		public virtual string SidToPrincipalName(byte[] sid)
		{
			return this.WindowsAuthenticationInternal.SidToPrincipalName(sid);
		}

		#endregion
	}
}