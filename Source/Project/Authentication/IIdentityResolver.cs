using System.Collections.Generic;
using System.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Authentication
{
	public interface IIdentityResolver
	{
		#region Methods

		IIdentity GetIdentity(IDictionary<string, string> cookies);

		#endregion
	}
}