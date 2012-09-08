using System;
using System.Data;
using System.Data.Common;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageTransaction : DbTransaction
	{
		private readonly TableStorageConnection m_Connection;
		private readonly IsolationLevel m_IsolationLevel;

		internal TableStorageTransaction(TableStorageConnection connection, IsolationLevel isolationLevel)
		{
			m_Connection = connection;
			m_IsolationLevel = isolationLevel;
		}

		protected override DbConnection DbConnection
		{
			get { return m_Connection; }
		}

		public override IsolationLevel IsolationLevel
		{
			get { return m_IsolationLevel; }
		}

		public override void Commit()
		{
			throw new NotImplementedException();
		}

		public override void Rollback()
		{
			throw new NotImplementedException();
		}
	}
}