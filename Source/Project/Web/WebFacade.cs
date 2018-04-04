using System.Security.Principal;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class WebFacade : IWebFacade
	{
		#region Properties

		public virtual HttpContextBase Context => HttpContext.Current != null ? new HttpContextWrapper(HttpContext.Current) : null;
		public virtual HttpRequestBase Request => this.Context?.Request;
		public virtual HttpResponseBase Response => this.Context?.Response;
		public virtual IPrincipal User => this.Context?.User;

		#endregion
	}
}