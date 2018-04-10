using System;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace RegionOrebroLan.ReportingServices.Security.Principal
{
	[Serializable]
	public class WindowsFederationPrincipal : ClaimsPrincipal, IWindowsFederationPrincipal
	{
		#region Constructors

		// ReSharper disable SuggestBaseTypeForParameter
		public WindowsFederationPrincipal(IWindowsFederationIdentity identity) : base(identity) { }
		// ReSharper restore SuggestBaseTypeForParameter

		protected WindowsFederationPrincipal(SerializationInfo info, StreamingContext context) : base(info, context) { }

		#endregion

		#region Properties

		IWindowsFederationIdentity IWindowsFederationPrincipal.Identity => (IWindowsFederationIdentity) this.Identity;

		#endregion
	}
}