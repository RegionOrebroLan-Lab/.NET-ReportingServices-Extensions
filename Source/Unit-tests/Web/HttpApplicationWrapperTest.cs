using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RegionOrebroLan.ReportingServices.Web;

namespace RegionOrebroLan.ReportingServices.UnitTests.Web
{
	[TestClass]
	public class HttpApplicationWrapperTest
	{
		#region Methods

		[TestMethod]
		public void ImplicitOperator_IfTheHttpApplicationParameterIsNotNull_ShouldWrapTheParameter()
		{
			var httpApplication = new HttpApplication();
			var httpApplicationWrapper = (HttpApplicationWrapper) httpApplication;
			Assert.AreEqual(httpApplication, httpApplicationWrapper.WrappedInstance);
		}

		[TestMethod]
		public void ImplicitOperator_IfTheHttpApplicationParameterIsNull_ShouldReturnNull()
		{
			Assert.IsNull((HttpApplicationWrapper) null);
		}

		[TestMethod]
		public void Modules_ShouldReturnTheModules()
		{
			var modules = new Dictionary<string, IHttpModule>
			{
				{"First", Mock.Of<IHttpModule>()},
				{"Second", Mock.Of<IHttpModule>()},
				{"Third", Mock.Of<IHttpModule>()}
			};

			var httpApplication = new HttpApplication();
			var addModuleMethod = typeof(HttpModuleCollection).GetMethod("AddModule", BindingFlags.Instance | BindingFlags.NonPublic);

			foreach(var module in modules)
			{
				// ReSharper disable PossibleNullReferenceException
				addModuleMethod.Invoke(httpApplication.Modules, new object[] {module.Key, module.Value});
				// ReSharper restore PossibleNullReferenceException
			}

			var httpApplicationWrapper = new HttpApplicationWrapper(httpApplication);

			Assert.AreEqual(modules.Count, httpApplicationWrapper.Modules.Count);

			for(var i = 0; i < modules.Count; i++)
			{
				Assert.AreEqual(modules.ElementAt(i).Key, httpApplicationWrapper.Modules.ElementAt(i).Key);
				Assert.AreEqual(modules.ElementAt(i).Value, httpApplicationWrapper.Modules.ElementAt(i).Value);
			}
		}

		#endregion
	}
}