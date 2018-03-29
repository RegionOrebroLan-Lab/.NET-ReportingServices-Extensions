namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public interface IFormsAuthenticationTicketFactory
	{
		#region Methods

		IFormsAuthenticationTicket Create(string name);
		IFormsAuthenticationTicket Create(string name, bool persistent);

		#endregion
	}
}