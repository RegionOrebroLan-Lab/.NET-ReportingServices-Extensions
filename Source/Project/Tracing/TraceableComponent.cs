using System;
using System.Diagnostics;
using Microsoft.ReportingServices.Extensions;

namespace RegionOrebroLan.ReportingServices.Tracing
{
	public abstract class TraceableComponent : ITraceableComponent, ITraceLog
	{
		#region Properties

		public virtual bool TraceError => this.TraceLog?.TraceError ?? false;
		public virtual bool TraceInfo => this.TraceLog?.TraceInfo ?? false;
		protected internal virtual ITraceLog TraceLog { get; set; }
		public virtual bool TraceWarning => this.TraceLog?.TraceWarning ?? false;
		public virtual bool TraceVerbose => this.TraceLog?.TraceVerbose ?? false;

		#endregion

		#region Methods

		protected internal virtual void HandleException(Exception exception)
		{
			if(exception == null)
				return;

			if(this.TraceLog != null)
				this.WriteTrace(exception.ToString(), TraceLevel.Error);

			throw exception;
		}

		public virtual void SetTraceLog(ITraceLog traceLog)
		{
			this.TraceLog = traceLog;
		}

		public virtual void WriteTrace(string message, TraceLevel level)
		{
			if(this.TraceLog == null)
				throw new InvalidOperationException(this.GetType().FullName + ": The trace-log is not set.");

			this.TraceLog.WriteTrace(this.GetType().FullName + ": " + message, level);
		}

		#endregion
	}
}