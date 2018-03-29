namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public interface IFormsAuthenticationTicket
	{
		#region Properties

		bool Expired { get; }
		string Name { get; }

		#endregion
	}
}