using System;
using System.Web.Configuration;
using RegionOrebroLan.ReportingServices.Abstractions;

namespace RegionOrebroLan.ReportingServices.Web.Configuration
{
	public class FormsAuthenticationConfigurationWrapper : Wrapper<FormsAuthenticationConfiguration>, IFormsAuthenticationConfiguration
	{
		#region Constructors

		public FormsAuthenticationConfigurationWrapper(FormsAuthenticationConfiguration formsAuthenticationConfiguration) : base(formsAuthenticationConfiguration, nameof(formsAuthenticationConfiguration)) { }

		#endregion

		#region Properties

		public virtual TimeSpan Timeout => this.WrappedInstance.Timeout;

		#endregion
	}
}