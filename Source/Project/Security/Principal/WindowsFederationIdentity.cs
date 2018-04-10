using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	[Serializable]
	public class WindowsFederationIdentity : ClaimsIdentity, IWindowsFederationIdentity
	{
		#region Constructors

		public WindowsFederationIdentity(IEnumerable<Claim> claims) : base(claims, "Federation") { }
		protected WindowsFederationIdentity(SerializationInfo info, StreamingContext context) : base(info, context) { }

		#endregion

		#region Properties

		public virtual string UserPrincipalName => this.Claims.FirstOrDefault(claim => string.Equals(claim.Type, ClaimTypes.Upn, StringComparison.OrdinalIgnoreCase))?.Value;

		#endregion
	}
}