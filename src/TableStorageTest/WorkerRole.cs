using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Magurany.Data.TableStorageClient;
using Microsoft.WindowsAzure.StorageClient;

namespace Passageways.Airway.ResourceAccess.Settings
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.WriteLine("WorkerRole1 entry point called", "Information");

			string connectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
			CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);

			CloudTableClient tableClient = account.CreateCloudTableClient();
			tableClient.DeleteTableIfExist("MyTable");

			SelectExample();
			SelectExampleAdapter();
			InsertExample();
			InsertExampleAdapter();
			UpdateExample();
			UpdateExampleAdapter();
			DeleteExample();
			DeleteExampleAdapter();

			while(true)
			{
				Thread.Sleep(10000);
				Trace.WriteLine("Working", "Information");
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			// For information on handling configuration changes
			// see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

			return base.OnStart();
		}

		private void SelectExample()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand selectCommand = connection.CreateCommand("GET /MyTable()?$filter=PartitionKey eq @PartitionKey");
				selectCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");

				using(DbDataReader reader = selectCommand.ExecuteReader())
				{
					while(reader.Read())
					{
						// Do something awesome
					}
				}
			}
		}

		private void SelectExampleAdapter()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand selectCommand = connection.CreateCommand("GET /MyTable()?$filter=PartitionKey eq @PartitionKey");
				selectCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");

				DataTable table = new DataTable();

				DbDataAdapter adapter = factory.CreateDataAdapter();
				adapter.SelectCommand = selectCommand;
				adapter.Fill(table);
			}
		}

		private static void InsertExample()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand insertCommand = connection.CreateCommand("POST /MyTable()");
				insertCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");
				insertCommand.AddParameter("RowKey", DbType.String, "RowKey1");
				insertCommand.AddParameter("MyField", DbType.String, "Some Data");
				insertCommand.ExecuteNonQuery();
			}
		}

		private static void InsertExampleAdapter()
		{
			DataTable table = new DataTable();
			table.Columns.Add("PartitionKey");
			table.Columns.Add("RowKey");
			table.Columns.Add("MyField");

			DataRow row = table.NewRow();
			row["PartitionKey"] = "MyPartitionKey";
			row["RowKey"] = "RowKey2";
			row["MyField"] = "Some Data";

			table.Rows.Add(row);

			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand insertCommand = connection.CreateCommand("POST /MyTable()");
				insertCommand.AddMappedParameter("PartitionKey", DbType.String, "PartitionKey");
				insertCommand.AddMappedParameter("RowKey", DbType.Guid, "RowKey");
				insertCommand.AddMappedParameter("MyField", DbType.String, "MyField");

				DbDataAdapter adapter = factory.CreateDataAdapter();
				adapter.InsertCommand = insertCommand;
				adapter.Update(table);
			}
		}

		private static void UpdateExample()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand updateCommand = connection.CreateCommand("PUT /MyTable(PartitionKey=@PartitionKey,RowKey=@RowKey)");
				updateCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");
				updateCommand.AddParameter("RowKey", DbType.String, "RowKey1");
				updateCommand.AddParameter("MyField", DbType.String, "Some Data Updated");
				updateCommand.ExecuteNonQuery();
			}
		}

		private static void UpdateExampleAdapter()
		{
			DataTable table = new DataTable();
			table.Columns.Add("PartitionKey");
			table.Columns.Add("RowKey");
			table.Columns.Add("MyField");

			DataRow row = table.NewRow();
			row["PartitionKey"] = "MyPartitionKey";
			row["RowKey"] = "RowKey2";
			row["MyField"] = "Some Data";

			table.Rows.Add(row);
			table.AcceptChanges();

			row["MyField"] = "Some Data Updated";

			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand updateCommand = connection.CreateCommand("PUT /MyTable(PartitionKey=@PartitionKey,RowKey=@RowKey)");
				updateCommand.AddMappedParameter("PartitionKey", DbType.String, "PartitionKey");
				updateCommand.AddMappedParameter("RowKey", DbType.String, "RowKey");
				updateCommand.AddMappedParameter("MyField", DbType.String, "MyField");

				DbDataAdapter adapter = factory.CreateDataAdapter();
				adapter.UpdateCommand = updateCommand;
				adapter.Update(table);
			}
		}

		private static void DeleteExample()
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand deleteCommand = connection.CreateCommand("DELETE /MyTable(PartitionKey=@PartitionKey,RowKey=@RowKey)");
				deleteCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");
				deleteCommand.AddParameter("RowKey", DbType.String, "RowKey1");
				deleteCommand.ExecuteNonQuery();
			}
		}

		private static void DeleteExampleAdapter()
		{
			DataTable table = new DataTable();
			table.Columns.Add("PartitionKey");
			table.Columns.Add("RowKey");
			table.Columns.Add("MyField");

			DataRow row = table.NewRow();
			row["PartitionKey"] = "MyPartitionKey";
			row["RowKey"] = "RowKey2";
			row["MyField"] = "Some Data";

			table.Rows.Add(row);
			table.AcceptChanges();

			row.Delete();

			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				connection.ConnectionString = CloudConfigurationManager.GetSetting("StorageConnectionString");
				connection.Open();

				DbCommand deleteCommand = connection.CreateCommand("DELETE /MyTable(PartitionKey=@PartitionKey,RowKey=@RowKey)");
				deleteCommand.AddMappedParameter("PartitionKey", DbType.String, "PartitionKey");
				deleteCommand.AddMappedParameter("RowKey", DbType.String, "RowKey");

				DbDataAdapter adapter = factory.CreateDataAdapter();
				adapter.DeleteCommand = deleteCommand;
				adapter.Update(table);
			}
		}
	}
}
