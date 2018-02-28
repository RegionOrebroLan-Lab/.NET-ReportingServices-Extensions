using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public interface IWebContext
	{
		#region Properties

		HttpContextBase HttpContext { get; }

		#endregion
	}
}