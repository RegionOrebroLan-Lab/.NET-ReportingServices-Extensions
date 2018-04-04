using System.Security.Principal;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public interface IWebFacade
	{
		#region Properties

		HttpContextBase Context { get; }
		HttpRequestBase Request { get; }
		HttpResponseBase Response { get; }
		IPrincipal User { get; }

		#endregion
	}
}