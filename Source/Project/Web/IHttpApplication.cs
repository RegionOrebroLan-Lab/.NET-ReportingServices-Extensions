using System.Collections.Generic;
using System.Web;

namespace RegionOrebroLan.ReportingServices.Web
{
	public interface IHttpApplication
	{
		#region Properties

		HttpContextBase Context { get; }
		IReadOnlyDictionary<string, IHttpModule> Modules { get; }

		#endregion
	}
}