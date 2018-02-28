using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class WebContext : IWebContext
	{
		#region Properties

		public virtual HttpContextBase HttpContext => System.Web.HttpContext.Current != null ? new HttpContextWrapper(System.Web.HttpContext.Current) : null;

		#endregion
	}
}