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

			this.Response.Clear();

			if(this.User == null)
			{
				this.Response.Write("Not signed id.");
				//throw new InvalidOperationException("Not signed in.");

				return;
			}

			if(!(this.User.Identity is WindowsIdentity))
			{
				this.Response.Write("<p>Not signed in as windows-user.</p>");
				//throw new InvalidOperationException("Not signed in as windows-user.");

				return;
			}

			this.Response.Write("<p>You are signed in as " + this.User.Identity.Name + "<p>");

			this.Response.Write("<p>" + this.Request.Url + "</p>");

			var returnUrl = this.Request.QueryString["ReturnUrl"];

			if(Uri.TryCreate(returnUrl, UriKind.RelativeOrAbsolute, out var result))
			{
				this.Response.Redirect(result.IsAbsoluteUri ? result.AbsoluteUri : result.OriginalString, false);

				return;
			}

			var uriBuilder = new UriBuilder(this.Request.Url) {Query = string.Empty};
			uriBuilder.Path = uriBuilder.Path.Replace("Federation.aspx", string.Empty);

			this.Response.Redirect(uriBuilder.Uri.ToString(), false);
		}

		#endregion
	}
}