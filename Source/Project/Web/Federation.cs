using System;
using System.Security.Principal;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class Federation : System.Web.UI.Page
	{
		#region Methods

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if(this.User == null)
				throw new InvalidOperationException("Not signed in.");

			if(!(this.User.Identity is WindowsIdentity))
				throw new InvalidOperationException("Not signed in as windows-user.");

			var returnUrl = this.Request.QueryString["ReturnUrl"];

			if(Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out var result))
				this.Response.Redirect(result.IsAbsoluteUri ? result.AbsoluteUri : result.OriginalString, true);

			this.Response.Redirect(this.Request.RawUrl.Replace("Federation.aspx", string.Empty), true);
		}

		#endregion
	}
}