using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using log4net;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using RegionOrebroLan.ReportingServices.Security.Principal;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public class WindowsAuthentication : IWindowsAuthenticationExtension2
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(WindowsAuthentication));

		#endregion

		#region Constructors

		public WindowsAuthentication() : this(ServiceLocator.Instance.GetService<IIdentityResolver>(), _log, ServiceLocator.Instance.GetService<IWebFacade>(), ServiceLocator.Instance.GetService<IWindowsAuthenticationExtension2>("Internal")) { }

		public WindowsAuthentication(IIdentityResolver identityResolver, ILog log, IWebFacade webFacade, IWindowsAuthenticationExtension2 windowsAuthenticationInternal)
		{
			this.IdentityResolver = identityResolver ?? throw new ArgumentNullException(nameof(identityResolver));
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WebFacade = webFacade ?? throw new ArgumentNullException(nameof(webFacade));
			this.WindowsAuthenticationInternal = windowsAuthenticationInternal ?? throw new ArgumentNullException(nameof(windowsAuthenticationInternal));
		}

		#endregion

		#region Properties

		protected internal virtual IIdentityResolver IdentityResolver { get; }
		public virtual string LocalizedName => null;
		protected internal virtual ILog Log { get; }
		protected internal virtual IWebFacade WebFacade { get; }
		protected internal virtual IWindowsAuthenticationExtension2 WindowsAuthenticationInternal { get; }

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
			userIdentity = this.WebFacade.User?.Identity;

			if(userIdentity == null || !userIdentity.IsAuthenticated)
			{
				var message = string.Format(CultureInfo.InvariantCulture, "The http-context-user-identity can not be \"{0}\".", this.GetIdentityName(userIdentity));

				if(this.WebFacade.Context == null)
					message += " The http-context is null.";
				else if(this.WebFacade.User == null)
					message += " The http-context-user is null.";

				const string method = "GetUserInfo (2 parameters)";

				this.LogErrorIfEnabled(message, method);

				throw new InvalidOperationException("WindowsAuthentication - " + method + ": " + message);
			}

			if(!(userIdentity is IWindowsFederationIdentity))
				this.ThrowIdentityMustBeAWindowsFederationIdentityException("GetUserInfo (2 parameters)", "http-context-user-identity");
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

			if(!(userIdentity is IWindowsFederationIdentity))
				this.ThrowIdentityMustBeAWindowsFederationIdentityException("GetUserInfo (3 parameters)", "request-context-user");
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

		public virtual void SetConfiguration(string configuration) { }

		public virtual string SidToPrincipalName(byte[] sid)
		{
			return this.WindowsAuthenticationInternal.SidToPrincipalName(sid);
		}

		protected internal virtual void ThrowIdentityMustBeAWindowsFederationIdentityException(string method, string parameterName)
		{
			var message = string.Format(CultureInfo.InvariantCulture, "The {0} must be a windows-federation-identity.", parameterName);

			this.LogErrorIfEnabled(message, method);

			throw new InvalidOperationException("WindowsAuthentication - " + method + ": " + message);
		}

		#endregion
	}
}