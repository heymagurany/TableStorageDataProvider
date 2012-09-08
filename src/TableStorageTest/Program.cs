using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
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
				connection.ConnectionString = "UseDevelopmentStorage=true";
				connection.Open();

				DbCommand selectCommand = connection.CreateCommand();
				selectCommand.CommandText = "GET /Test()";

				using(DbDataReader reader = selectCommand.ExecuteReader())
				{
					while(reader.Read())
					{
						Console.WriteLine("asdf");
					}
				}
			}
		}
	}
}