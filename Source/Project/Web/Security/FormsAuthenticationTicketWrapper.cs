using System.Web.Security;
using RegionOrebroLan.ReportingServices.Abstractions;

namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public class FormsAuthenticationTicketWrapper : Wrapper<FormsAuthenticationTicket>, IFormsAuthenticationTicket
	{
		#region Constructors

		public FormsAuthenticationTicketWrapper(FormsAuthenticationTicket formsAuthenticationTicket) : base(formsAuthenticationTicket, nameof(formsAuthenticationTicket)) { }

		#endregion

		#region Properties

		public virtual bool Expired => this.WrappedInstance.Expired;
		public virtual string Name => this.WrappedInstance.Name;

		#endregion

		#region Methods

		#region Implicit operator

		public static implicit operator FormsAuthenticationTicketWrapper(FormsAuthenticationTicket formsAuthenticationTicket)
		{
			return formsAuthenticationTicket != null ? new FormsAuthenticationTicketWrapper(formsAuthenticationTicket) : null;
		}

		#endregion

		public static FormsAuthenticationTicketWrapper ToFormsAuthenticationTicketWrapper(FormsAuthenticationTicket formsAuthenticationTicket)
		{
			return formsAuthenticationTicket;
		}

		#endregion
	}
}