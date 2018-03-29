using System;
using System.Globalization;
using System.Web.Security;
using RegionOrebroLan.ReportingServices.Abstractions;

namespace RegionOrebroLan.ReportingServices.Web.Security
{
	public class FormsAuthenticationWrapper : IFormsAuthentication
	{
		#region Properties

		public virtual string CookieName => FormsAuthentication.FormsCookieName;

		#endregion

		#region Methods

		public IFormsAuthenticationTicket Decrypt(string value) => (FormsAuthenticationTicketWrapper) FormsAuthentication.Decrypt(value);

		public virtual string Encrypt(IFormsAuthenticationTicket ticket)
		{
			if(ticket == null)
				throw new ArgumentNullException(nameof(ticket));

			if(!(ticket is IWrapper<FormsAuthenticationTicket> ticketWrapper))
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "The ticket must implement \"{0}\".", typeof(IWrapper<FormsAuthenticationTicket>)), nameof(ticket));

			return FormsAuthentication.Encrypt(ticketWrapper.WrappedInstance);
		}

		#endregion
	}
}