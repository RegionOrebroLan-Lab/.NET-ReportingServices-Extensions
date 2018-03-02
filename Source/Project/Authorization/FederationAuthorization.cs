using System;
using System.Collections.Specialized;
using Microsoft.ReportingServices.Authorization;
using Microsoft.ReportingServices.Interfaces;
using RegionOrebroLan.ReportingServices.Tracing;

namespace RegionOrebroLan.ReportingServices.Authorization
{
	public class FederationAuthorization : TraceableComponent, IAuthorizationExtension
	{
		#region Constructors

		public FederationAuthorization()
		{
			this.WindowsAuthorization = new WindowsAuthorization();
		}

		#endregion

		#region Properties

		public virtual string LocalizedName => null;

		[CLSCompliant(false)]
		protected internal WindowsAuthorization WindowsAuthorization { get; }

		#endregion

		#region Methods

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, CatalogOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, CatalogOperation[] requiredOperations)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperations);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ReportOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, FolderOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, FolderOperation[] requiredOperations)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperations);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ResourceOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ResourceOperation[] requiredOperations)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperations);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, DatasourceOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ModelOperation requiredOperation)
		{
			return true;
			//return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual bool CheckAccess(string userName, IntPtr userToken, byte[] secDesc, ModelItemOperation requiredOperation)
		{
			return this.WindowsAuthorization.CheckAccess(userName, userToken, secDesc, requiredOperation);
		}

		public virtual byte[] CreateSecurityDescriptor(AceCollection acl, SecurityItemType itemType, out string stringSecDesc)
		{
			return this.WindowsAuthorization.CreateSecurityDescriptor(acl, itemType, out stringSecDesc);
		}

		public virtual StringCollection GetPermissions(string userName, IntPtr userToken, SecurityItemType itemType, byte[] secDesc)
		{
			return this.WindowsAuthorization.GetPermissions(userName, userToken, itemType, secDesc);
		}

		public virtual void SetConfiguration(string configuration) { }

		#endregion
	}
}