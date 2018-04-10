using System.Security.Claims;
using System.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	public class WindowsFederationIdentityFactory : IWindowsFederationIdentityFactory
	{
		#region Methods

		public virtual IWindowsFederationIdentity Create(string userPrincipalName)
		{
			var windowsIdentity = new WindowsIdentity(userPrincipalName);

			return new WindowsFederationIdentity(new[]
			{
				new Claim(ClaimTypes.Name, windowsIdentity.Name),
				new Claim(ClaimTypes.Upn, userPrincipalName)
			});
		}

		#endregion
	}
}