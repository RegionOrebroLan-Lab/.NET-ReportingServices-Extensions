using System.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	public interface IWindowsFederationIdentity : IIdentity
	{
		#region Properties

		string UserPrincipalName { get; }

		#endregion
	}
}