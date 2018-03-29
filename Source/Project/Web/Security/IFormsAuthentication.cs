namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public interface IFormsAuthentication
	{
		#region Properties

		string CookieName { get; }

		#endregion

		#region Methods

		IFormsAuthenticationTicket Decrypt(string value);
		string Encrypt(IFormsAuthenticationTicket ticket);

		#endregion
	}
}