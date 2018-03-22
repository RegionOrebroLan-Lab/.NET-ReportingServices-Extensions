namespace RegionOrebroLan.ReportingServices.Authentication
{
	public interface IIdentityResolverFactory
	{
		#region Methods

		IIdentityResolver Create(string configuration);

		#endregion
	}
}