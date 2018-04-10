using System;
using System.Linq;
using System.Security.Claims;
using log4net;
using RegionOrebroLan.ReportingServices.InversionOfControl;
using RegionOrebroLan.ReportingServices.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Security.Claims
{
	public class WindowsFederationClaimsAuthenticationManager : ClaimsAuthenticationManager
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(WindowsFederationClaimsAuthenticationManager));

		#endregion

		#region Constructors

		public WindowsFederationClaimsAuthenticationManager() : this(_log, ServiceLocator.Instance.GetService<IWindowsFederationIdentityFactory>()) { }

		public WindowsFederationClaimsAuthenticationManager(ILog log, IWindowsFederationIdentityFactory windowsFederationIdentityFactory)
		{
			this.Log = log ?? throw new ArgumentNullException(nameof(log));
			this.WindowsFederationIdentityFactory = windowsFederationIdentityFactory ?? throw new ArgumentNullException(nameof(windowsFederationIdentityFactory));
		}

		#endregion

		#region Properties

		protected internal virtual ILog Log { get; }
		protected internal virtual IWindowsFederationIdentityFactory WindowsFederationIdentityFactory { get; }

		#endregion

		#region Methods

		public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
		{
			try
			{
				var upnClaim = incomingPrincipal.Claims.FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.Upn, StringComparison.OrdinalIgnoreCase));

				if(upnClaim == null)
					throw new InvalidOperationException("The principal must have an UPN-claim.");

				return new WindowsFederationPrincipal(this.WindowsFederationIdentityFactory.Create(upnClaim.Value));
			}
			catch(Exception exception)
			{
				if(this.Log.IsErrorEnabled)
					this.Log.Error(exception);

				throw;
			}
		}

		#endregion
	}
}