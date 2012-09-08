using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Security.Permissions;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageClientFactory : DbProviderFactory
	{
		[SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is required by the data provider infrastructure.")]
		[SuppressMessage("Passageways.NamingRules", "PW0001:PrivateNamingConventions", Justification = "The field is not private, but static and read-only, so it's okay for it to be public.")]
		public static readonly TableStorageClientFactory Instance = new TableStorageClientFactory();

		private TableStorageClientFactory() { }

		public override DbCommand CreateCommand()
		{
			return new TableStorageCommand();
		}

		public override DbCommandBuilder CreateCommandBuilder()
		{
			throw new NotSupportedException();
		}

		public override DbConnection CreateConnection()
		{
			return new TableStorageConnection();
		}

		public override DbConnectionStringBuilder CreateConnectionStringBuilder()
		{
			throw new NotSupportedException();
		}

		public override DbDataAdapter CreateDataAdapter()
		{
			return new TableStorageDataAdapter();
		}

		public override DbDataSourceEnumerator CreateDataSourceEnumerator()
		{
			throw new NotSupportedException();
		}

		public override DbParameter CreateParameter()
		{
			return new TableStorageParameter();
		}

		public override CodeAccessPermission CreatePermission(PermissionState state)
		{
			throw new NotSupportedException();
		}
	}
}