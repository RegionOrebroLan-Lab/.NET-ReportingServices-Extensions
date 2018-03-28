using System;

namespace RegionOrebroLan.ReportingServices.Extensions
{
	public static class UriExtension
	{
		#region Methods

		public static string ToStringValue(this Uri url)
		{
			if(url == null)
				return null;

			return url.IsAbsoluteUri ? url.ToString() : url.OriginalString;
		}

		#endregion
	}
}