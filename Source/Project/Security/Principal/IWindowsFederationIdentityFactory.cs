namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	public interface IWindowsFederationIdentityFactory
	{
		#region Methods

		IWindowsFederationIdentity Create(string userPrincipalName);

		#endregion
	}
}