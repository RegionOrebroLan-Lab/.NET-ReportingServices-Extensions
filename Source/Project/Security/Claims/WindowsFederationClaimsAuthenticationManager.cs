using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using RegionOrebroLan.ReportingServices.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Security.Claims
{
	public class WindowsFederationClaimsAuthenticationManager : ClaimsAuthenticationManager
	{
		#region Methods

		public override ClaimsPrincipal Authenticate(string resourceName, ClaimsPrincipal incomingPrincipal)
		{
			var upnClaim = incomingPrincipal.Claims.FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.Upn, StringComparison.OrdinalIgnoreCase));

			if(upnClaim == null)
				throw new InvalidOperationException("The principal must have an UPN-claim.");

			var windowsIdentity = new WindowsIdentity(upnClaim.Value);

			return new WindowsFederationPrincipal(new WindowsFederationIdentity(new[]
			{
				new Claim(ClaimTypes.Name, windowsIdentity.Name),
				upnClaim
			}));
		}

		#endregion
	}
}