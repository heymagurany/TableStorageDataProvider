using System;
using System.Data;
using System.Data.Common;
using Microsoft.WindowsAzure;

namespace Magurany.Data.TableStorageClient.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			DbProviderFactory factory = DbProviderFactories.GetFactory("Magurany.Data.TableStorageClient");

			using(DbConnection connection = factory.CreateConnection())
			{
				//CloudConfigurationManager.GetSetting("");

				connection.ConnectionString = "UseDevelopmentStorage=true";
				connection.Open();

				DbCommand selectCommand = connection.CreateCommand();
				selectCommand.CommandText = "GET /MyTable()$PartitionKey eq @PartitionKey";
				selectCommand.AddParameter("PartitionKey", DbType.String, "MyPartitionKey");

				using(DbDataReader reader = selectCommand.ExecuteReader())
				{
					while(reader.Read())
					{
						// Do something awesome
					}
				}

				DataTable table = new DataTable();

				DbDataAdapter adapter = factory.CreateDataAdapter();
				adapter.SelectCommand = selectCommand;
				adapter.Fill(table);
			}
		}
	}
}