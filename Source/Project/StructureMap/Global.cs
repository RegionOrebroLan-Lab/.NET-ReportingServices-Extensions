using System.Diagnostics.CodeAnalysis;
using System.Web.Configuration;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Authentication;
using RegionOrebroLan.ReportingServices.Web;
using RegionOrebroLan.ReportingServices.Web.Configuration;
using RegionOrebroLan.ReportingServices.Web.Security;
using StructureMap;
using ReportingServicesWindowsAuthentication = Microsoft.ReportingServices.Authentication.WindowsAuthentication;
using WebContext = RegionOrebroLan.ReportingServices.Web.WebContext;

namespace RegionOrebroLan.ReportingServices.StructureMap
{
	[SuppressMessage("Microsoft.Naming", "CA1716: Identifiers should not match keywords.")]
	public static class Global
	{
		#region Fields

		private static volatile IContainer _container;
		private static readonly object _lock = new object();

		#endregion

		#region Properties

		public static IContainer Container
		{
			get
			{
				// ReSharper disable InvertIf
				if(_container == null)
				{
					lock(_lock)
					{
						if(_container == null)
						{
							_container = new Container(configuration =>
							{
								configuration.For<IFormsAuthentication>().Singleton().Use<FormsAuthenticationWrapper>();
								configuration.For<IFormsAuthenticationConfiguration>().Singleton().Use<FormsAuthenticationConfigurationWrapper>();
								configuration.For<IFormsAuthenticationTicketFactory>().Singleton().Use<FormsAuthenticationTicketFactory>();
								configuration.For<IIdentityResolver>().Singleton().Use<IdentityResolver>();
								configuration.For<IRedirectInformationFactory>().Singleton().Use<RedirectInformationFactory>();
								configuration.For<IWebContext>().Singleton().Use<WebContext>();
								configuration.For<IWindowsAuthenticationExtension2>().Use<ReportingServicesWindowsAuthentication>().Name = "Internal";
								configuration.For<FormsAuthenticationConfiguration>().Singleton().Use<FormsAuthenticationConfiguration>();
							});
						}
					}
				}
				// ReSharper restore InvertIf

				return _container;
			}
			set
			{
				if(value == _container)
					return;

				lock(_lock)
				{
					_container = value;
				}
			}
		}

		#endregion
	}
}