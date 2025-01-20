using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using Serilog;
using System.Runtime.CompilerServices;

namespace WebApplication.DAL
{
	public class SchoolInterceptorLogging : DbCommandInterceptor
	{
		private ILogger _logger = Log.ForContext<SchoolInterceptorLogging>();
		private readonly Stopwatch _stopwatch = new Stopwatch();

		public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			base.ScalarExecuting(command, interceptionContext);
			_stopwatch.Restart();
		}

		public override void ScalarExecuted(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
		{
			_stopwatch.Stop();
			if (interceptionContext.Exception != null)
			{
				_logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
			}
			else
			{
				Trace(command);

			}
			base.ScalarExecuted(command, interceptionContext);
		}

		public override void NonQueryExecuting(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			base.NonQueryExecuting(command, interceptionContext);
			_stopwatch.Restart();
		}

		public override void NonQueryExecuted(DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
		{
			_stopwatch.Stop();
			if (interceptionContext.Exception != null)
			{
				_logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
			}
			else
			{
				Trace(command);
			}
			base.NonQueryExecuted(command, interceptionContext);
		}

		public override void ReaderExecuting(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			base.ReaderExecuting(command, interceptionContext);
			_stopwatch.Restart();
		}
		public override void ReaderExecuted(DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
		{
			_stopwatch.Stop();
			if (interceptionContext.Exception != null)
			{
				_logger.Error(interceptionContext.Exception, "Error executing command: {0}", command.CommandText);
			}
			else
			{
				Trace(command);
			}
			base.ReaderExecuted(command, interceptionContext);
		}

		private void Trace(DbCommand command, [CallerMemberName]  string method = null)
		{
			_logger.Verbose("SchoolInterceptor.{SqlQueryType} in time '{Elapsed}' and command {Command}", method, _stopwatch.Elapsed, command.CommandText);
		}
	}
}