using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using RegionOrebroLan.ReportingServices.Abstractions;

namespace RegionOrebroLan.ReportingServices.Web
{
	public class HttpApplicationWrapper : Wrapper<HttpApplication>, IHttpApplication
	{
		#region Constructors

		public HttpApplicationWrapper(HttpApplication httpApplication) : base(httpApplication, nameof(httpApplication)) { }

		#endregion

		#region Properties

		public virtual HttpContextBase Context => new HttpContextWrapper(this.WrappedInstance.Context);

		public virtual IReadOnlyDictionary<string, IHttpModule> Modules
		{
			get { return new ReadOnlyDictionary<string, IHttpModule>(this.WrappedInstance.Modules.AllKeys.ToDictionary(key => key, key => this.WrappedInstance.Modules[key], StringComparer.OrdinalIgnoreCase)); }
		}

		#endregion

		#region Methods

		#region Implicit operators

		public static implicit operator HttpApplicationWrapper(HttpApplication httpApplication)
		{
			return httpApplication != null ? new HttpApplicationWrapper(httpApplication) : null;
		}

		#endregion

		public static HttpApplicationWrapper ToHttpApplicationWrapper(HttpApplication httpApplication)
		{
			return httpApplication;
		}

		#endregion
	}
}