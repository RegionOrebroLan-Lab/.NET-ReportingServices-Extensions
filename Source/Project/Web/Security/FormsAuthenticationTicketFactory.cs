using System;
using System.Web.Security;
using RegionOrebroLan.ReportingServices.Web.Configuration;

namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public class FormsAuthenticationTicketFactory : IFormsAuthenticationTicketFactory
	{
		#region Constructors

		public FormsAuthenticationTicketFactory(IFormsAuthenticationConfiguration formsAuthenticationConfiguration)
		{
			this.FormsAuthenticationConfiguration = formsAuthenticationConfiguration ?? throw new ArgumentNullException(nameof(formsAuthenticationConfiguration));
		}

		#endregion

		#region Properties

		protected internal virtual IFormsAuthenticationConfiguration FormsAuthenticationConfiguration { get; }

		#endregion

		#region Methods

		public virtual IFormsAuthenticationTicket Create(string name)
		{
			return this.Create(name, false);
		}

		public virtual IFormsAuthenticationTicket Create(string name, bool persistent)
		{
			return (FormsAuthenticationTicketWrapper) new FormsAuthenticationTicket(name, persistent, this.FormsAuthenticationConfiguration.Timeout.Minutes);
		}

		#endregion
	}
}