using System.Data.Common;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageDataAdapter : DbDataAdapter
	{
		private int m_UpdateBatchSize;
		private const int MAX_BATCH_SIZE = 100;

		public TableStorageDataAdapter()
		{
			m_UpdateBatchSize = MAX_BATCH_SIZE;
		}

		public override int UpdateBatchSize
		{
			get { return m_UpdateBatchSize; }
			set { m_UpdateBatchSize = value; }
		}
	}
}