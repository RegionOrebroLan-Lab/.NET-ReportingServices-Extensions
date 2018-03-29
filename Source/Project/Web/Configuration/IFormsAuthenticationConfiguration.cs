using System;

namespace RegionOrebroLan.ReportingServices.Web.Configuration
{
	public interface IFormsAuthenticationConfiguration
	{
		#region Properties

		TimeSpan Timeout { get; }

		#endregion
	}
}