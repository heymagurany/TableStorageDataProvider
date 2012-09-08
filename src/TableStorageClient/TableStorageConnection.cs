using System;
using System.Data;
using System.Data.Common;
using Microsoft.WindowsAzure;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageConnection : DbConnection
	{
		private ConnectionState m_State;
		private string m_ConnectionString;
		private CloudStorageAccount m_StorageAccount;

		public TableStorageConnection()
		{
			m_State = ConnectionState.Closed;
		}

		public override string ConnectionString
		{
			get { return m_ConnectionString; }
			set { m_ConnectionString = value; }
		}

		public override string Database
		{
			get { return string.Empty; }
		}

		public override string DataSource
		{
			get { return StorageAccount.TableEndpoint.AbsoluteUri; }
		}

		public override string ServerVersion
		{
			get { return "2011-08-18"; }
		}

		public override ConnectionState State
		{
			get { return m_State; }
		}

		public CloudStorageAccount StorageAccount
		{
			get { return m_StorageAccount; }
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return new TableStorageTransaction(this, isolationLevel);
		}

		public override void ChangeDatabase(string databaseName)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			m_State = ConnectionState.Closed;
		}

		protected override DbCommand CreateDbCommand()
		{
			TableStorageCommand command = new TableStorageCommand();
			command.Connection = this;

			return command;
		}

		public override void Open()
		{
			m_StorageAccount = CloudStorageAccount.Parse(m_ConnectionString);
			m_State = ConnectionState.Open;

			// TODO (Matt Magurany 8/14/2012): Consider creating an HTTP request for batching here
		}
	}
}