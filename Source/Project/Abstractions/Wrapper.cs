using System;

namespace RegionOrebroLan.ReportingServices.Abstractions
{
	public abstract class Wrapper : IWrapper
	{
		#region Constructors

		protected Wrapper(object wrappedInstance, string wrappedInstanceParameterName)
		{
			this.WrappedInstance = wrappedInstance ?? throw new ArgumentNullException(wrappedInstanceParameterName);
		}

		#endregion

		#region Properties

		public virtual object WrappedInstance { get; }

		#endregion

		#region Methods

		public override bool Equals(object obj)
		{
			return this.WrappedInstance.Equals(obj is IWrapper wrapper ? wrapper.WrappedInstance : obj);
		}

		public override int GetHashCode()
		{
			return this.WrappedInstance.GetHashCode();
		}

		public override string ToString()
		{
			return this.WrappedInstance.ToString();
		}

		#endregion
	}

	public abstract class Wrapper<T> : Wrapper, IWrapper<T>
	{
		#region Constructors

		protected Wrapper(T wrappedInstance, string wrappedInstanceParameterName) : base(wrappedInstance, wrappedInstanceParameterName) { }

		#endregion

		#region Properties

		public new virtual T WrappedInstance => (T) base.WrappedInstance;

		#endregion
	}
}