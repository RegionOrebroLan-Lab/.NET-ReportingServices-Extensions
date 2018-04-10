namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	public interface IWindowsFederationPrincipal
	{
		#region Properties

		IWindowsFederationIdentity Identity { get; }

		#endregion
	}
}