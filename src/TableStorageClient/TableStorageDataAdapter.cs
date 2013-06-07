using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using Magurany.Data.TableStorageClient.Properties;

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
			get
			{
				return m_UpdateBatchSize;
			}
			set
			{
				if(value <= 0)
				{
					m_UpdateBatchSize = MAX_BATCH_SIZE;
				}
				else if (value < MAX_BATCH_SIZE)
				{
					m_UpdateBatchSize = value;
				}
				else
				{
					throw new ArgumentOutOfRangeException("value", value, string.Format(Resources.MaximumBatchSizeExceeded, MAX_BATCH_SIZE));
				}
			}
		}

		private int Fill(DataTable dataTable, DataTableMapping tableMapping, IDataReader dataReader, int startRecord, int maxRecords)
		{
			int rowsAffected = 0;
			int rowIndex = 0;

			dataTable.BeginLoadData();

			try
			{
				while(dataReader.Read() && ((rowsAffected < maxRecords) || maxRecords <= 0))
				{
					if(rowIndex >= startRecord)
					{
						DataRow row = dataTable.NewRow();

						for(int index = 0; index < dataReader.FieldCount; index++)
						{
							string name = dataReader.GetName(index);
							Type fieldType = dataReader.GetFieldType(index);
							DataColumn column = tableMapping.GetDataColumn(name, fieldType, dataTable, MissingMappingAction, MissingSchemaAction);

							if(column != null)
							{
								if(dataTable.Columns.Contains(column.ColumnName))
								{
									row[column] = dataReader.GetValue(index);
								}
							}
						}

						dataTable.Rows.Add(row);
						row.AcceptChanges();
						rowsAffected++;
					}

					rowIndex++;
				}
			}
			finally
			{
				dataTable.EndLoadData();
			}

			return rowsAffected;
		}

		protected override int Fill(DataSet dataSet, string srcTable, IDataReader dataReader, int startRecord, int maxRecords)
		{
			if(!dataReader.IsClosed)
			{
				DataTableMapping tableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(TableMappings, srcTable, srcTable, MissingMappingAction);

				if(tableMapping != null)
				{
					DataTable dataTable = tableMapping.GetDataTableBySchemaAction(dataSet, MissingSchemaAction);

					if(dataTable != null)
					{
						return Fill(dataTable, tableMapping, dataReader, startRecord, maxRecords);
					}
				}
			}

			return 0;
		}

		protected override int Fill(DataTable[] dataTables, IDataReader dataReader, int startRecord, int maxRecords)
		{
			int rowsAffected = 0;

			if(!dataReader.IsClosed)
			{
				bool moreResults = true;
				IEnumerator dataTableEnumerator = dataTables.GetEnumerator();

				while(moreResults && dataTableEnumerator.MoveNext())
				{
					DataTable dataTable = (DataTable)dataTableEnumerator.Current;
					DataTableMapping tableMapping = DataTableMappingCollection.GetTableMappingBySchemaAction(TableMappings, "Table", dataTable.TableName, MissingMappingAction);

					if(tableMapping != null)
					{
						rowsAffected += Fill(dataTable, tableMapping, dataReader, startRecord, maxRecords);
					}

					moreResults = dataReader.NextResult();
				}
			}

			return rowsAffected;
		}
	}
}