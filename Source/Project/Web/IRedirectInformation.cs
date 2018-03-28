using System;

namespace RegionOrebroLan.ReportingServices.Web
{
	public interface IRedirectInformation
	{
		#region Properties

		Exception Exception { get; }
		bool Redirect { get; }
		Uri Url { get; }

		#endregion
	}
}