using System;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class RedirectInformation : IRedirectInformation
	{
		#region Properties

		public virtual Exception Exception { get; set; }
		public virtual bool Redirect => this.Exception == null && this.Url != null;
		public virtual Uri Url { get; set; }

		#endregion
	}
}