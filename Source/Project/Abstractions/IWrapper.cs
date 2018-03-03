namespace RegionOrebroLan.ReportingServices.Abstractions
{
	public interface IWrapper
	{
		#region Properties

		object WrappedInstance { get; }

		#endregion
	}

	public interface IWrapper<out T> : IWrapper
	{
		#region Properties

		new T WrappedInstance { get; }

		#endregion
	}
}