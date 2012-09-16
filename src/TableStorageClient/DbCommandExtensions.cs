using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Magurany.Data.TableStorageClient
{
	[SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Following .NET Framework standards.")]
	public static class DbCommandExtensions
	{
		public static DbParameter AddMappedParameter(this DbCommand command, string parameterName, DbType dbType, string sourceColumn)
		{
			ThrowHelper.CheckArgumentNull(command, "command");

			DbParameter parameter = command.CreateParameter();
			parameter.DbType = dbType;
			parameter.ParameterName = parameterName;
			parameter.SourceColumn = sourceColumn;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public static DbParameter AddMappedParameter(this DbCommand command, string parameterName, DbType dbType, int size, string sourceColumn)
		{
			ThrowHelper.CheckArgumentNull(command, "command");

			DbParameter parameter = command.CreateParameter();
			parameter.DbType = dbType;
			parameter.ParameterName = parameterName;
			parameter.Size = size;
			parameter.SourceColumn = sourceColumn;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public static DbParameter AddParameter(this DbCommand command, string parameterName, DbType dbType, object value)
		{
			ThrowHelper.CheckArgumentNull(command, "command");

			DbParameter parameter = command.CreateParameter();
			parameter.DbType = dbType;
			parameter.ParameterName = parameterName;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}

		public static DbParameter AddParameter(this DbCommand command, string parameterName, DbType dbType, int size, object value)
		{
			ThrowHelper.CheckArgumentNull(command, "command");

			DbParameter parameter = command.CreateParameter();
			parameter.DbType = dbType;
			parameter.ParameterName = parameterName;
			parameter.Size = size;
			parameter.Value = value;

			command.Parameters.Add(parameter);

			return parameter;
		}
	}
}