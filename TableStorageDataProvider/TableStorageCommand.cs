using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using Magurany.Data.TableStorageClient.Properties;
using Microsoft.WindowsAzure.StorageClient;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageCommand : DbCommand
	{
		private TableStorageConnection m_Connection;
		private readonly TableStorageParameterCollection m_Parameters;

		public TableStorageCommand()
		{
			CommandTimeout = 30;
			CommandType = CommandType.Text;
			m_Parameters = new TableStorageParameterCollection();
		}

		public override void Prepare()
		{
			throw new NotSupportedException();
		}

		public override string CommandText { get; set; }

		public override int CommandTimeout { get; set; }

		public override CommandType CommandType { get; set; }

		public override UpdateRowSource UpdatedRowSource { get; set; }

		protected override DbConnection DbConnection
		{
			get
			{
				return m_Connection;
			}
			set
			{
				if((m_Connection != null) && m_Connection.State != ConnectionState.Closed)
				{
					throw new InvalidOperationException();
				}

				TableStorageConnection connection = value as TableStorageConnection;

				if(connection == null)
				{
					ThrowHelper.ThrowArgumentWrongType("value", typeof(TableStorageConnection));
				}

				m_Connection = connection;
			}
		}

		protected override DbParameterCollection DbParameterCollection
		{
			get { return m_Parameters; }
		}

		protected override DbTransaction DbTransaction { get; set; }

		public override bool DesignTimeVisible { get; set; }

		public override void Cancel()
		{
			throw new NotImplementedException();
		}

		private TableStorageFieldCollection ConvertParameters()
		{
			TableStorageFieldCollection parameters = new TableStorageFieldCollection();

			foreach(DbParameter parameter in m_Parameters)
			{
				if(parameter.Direction != ParameterDirection.Input)
				{
					throw new TableStorageException(string.Format(Resources.ParamterDirectionNotSupported, ParameterDirection.Input));
				}

				TableStorageField field = ToField(parameter);

				parameters.Add(field);
			}

			return parameters;
		}

		protected override DbParameter CreateDbParameter()
		{
			return new TableStorageParameter();
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			TableStorageFieldCollection parameters = ConvertParameters();
			HttpWebRequest request = GetRequest(parameters);

			try
			{
				WebResponse response = request.GetResponse();
				Stream responseStream = response.GetResponseStream();
				
				return new TableStorageDataReader(responseStream);
			}
			catch(WebException we)
			{
				HttpWebResponse response = (HttpWebResponse)we.Response;

				if(response == null)
				{
					throw new TableStorageException(we.Message, we);
				}
				
				if(response.StatusCode == HttpStatusCode.NotFound)
				{
					return new TableStorageDataReader(null);
				}

				throw TableStorageException.Parse(we);
			}
		}

		public override int ExecuteNonQuery()
		{
			TableStorageFieldCollection parameters = ConvertParameters();

			HttpWebRequest request = GetRequest(parameters);
			request.ContentType = "application/atom+xml";

			Stream requestStream = request.GetRequestStream();

			using(TableStorageDataWriter writer = new TableStorageDataWriter(requestStream))
			{
				// TODO (Matt Magurany 7/28/2012): Get the ID of the entry
				writer.WriteStartEntry(null);

				foreach(TableStorageField parameter in parameters)
				{
					writer.WriteField(parameter);
				}

				writer.WriteEndEntry();
				writer.Flush();
			}

			HttpWebResponse response = null;

			try
			{
				response = (HttpWebResponse)request.GetResponse();

				return 1;
			}
			catch(WebException we)
			{
				response = (HttpWebResponse)we.Response;

				if(response == null)
				{
					throw new TableStorageException(we.Message, we);
				}

				if(response.StatusCode == HttpStatusCode.NotFound)
				{
					return 0;
				}
				
				throw TableStorageException.Parse(we);
			}
			finally
			{
				if(response != null)
				{
					response.Close();
				}
			}
		}

		public override object ExecuteScalar()
		{
			throw new NotSupportedException();
		}

		private HttpWebRequest GetRequest(TableStorageFieldCollection parameters)
		{
			if(m_Connection.State != ConnectionState.Open)
			{
				throw new TableStorageException(Resources.ConnectionNotOpen);
			}

			Regex queryExpression = new Regex(@"^(?<verb>GET|POST|PUT|DELETE)\s*(?<path>/(?<tablename>[a-z][a-z0-9]{2,63}).*)$", RegexOptions.IgnoreCase);
			Match queryMatch = queryExpression.Match(CommandText);
			string verb = queryMatch.Groups["verb"].Value;
			string tableName = queryMatch.Groups["tablename"].Value;
			string path = queryMatch.Groups["path"].Value;
			Regex parametersExpression = new Regex(@"@(?<param>[\w]{1}[\w\d]*)", RegexOptions.IgnoreCase);

			MatchEvaluator evaluator = delegate(Match match)
			{
				string parameterName = match.Groups["param"].Value;

				if(parameters.Contains(parameterName))
				{
					TableStorageField parameter = parameters[parameterName];
					object value = parameter.Value;

					if((value == null) || DBNull.Value.Equals(value))
					{
						return string.Empty;
					}

					return string.Format(parameter.FormatString, parameter.Value);
				}

				throw new TableStorageException(string.Format(Resources.ParameterNotFound, parameterName));
			};

			path = parametersExpression.Replace(path, evaluator);

			CloudTableClient tableClient = m_Connection.StorageAccount.CreateCloudTableClient();
			tableClient.Timeout = TimeSpan.FromSeconds(CommandTimeout);
			tableClient.CreateTableIfNotExist(tableName);

			// TODO (Matt Magurany 8/14/2012): If a transaction exists, add to existing batch

			UriBuilder builder = new UriBuilder(tableClient.BaseUri);
			builder.Path = builder.Path.TrimEnd('/') + path;

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(builder.Uri);
			request.Accept = "application/atom+xml,application/xml";
			request.Headers["x-ms-version"] = m_Connection.ServerVersion;
			request.Headers["Accept-Charset"] = "UTF-8";
			request.Headers["DataServiceVersion"] = "2.0;NetFx";
			request.Headers["MaxDataServiceVersion"] = "2.0;NetFx";
			request.Method = verb;
			request.Timeout = CommandTimeout * 1000;

			m_Connection.StorageAccount.Credentials.SignRequestLite(request);

			return request;
		}

		private static TableStorageField ToField(DbParameter parameter)
		{
			string edmType;
			object value = parameter.Value;
			bool isNull;
			Type dataType;
			string formatString;

			if(value == null)
			{
				value = DBNull.Value;
				isNull = true;
			}
			else
			{
				isNull = false;
			}

			switch(parameter.DbType)
			{
				case DbType.Binary:
					edmType = TableStorageConstants.Edm.TYPE_BINARY;
					dataType = typeof(byte[]);

					byte[] binary = value as byte[];

					if(binary != null)
					{
						value = Convert.ToBase64String(binary);
					}
					formatString = "'{0}'";
					break;
				case DbType.Boolean:
					edmType = TableStorageConstants.Edm.TYPE_BOOLEAN;
					dataType = typeof(bool);
					formatString = "{0}";
					break;
				case DbType.DateTime:
					edmType = TableStorageConstants.Edm.TYPE_DATETIME;
					dataType = typeof(DateTime);
					formatString = "'{0:yyyy-MM-ddTHH-mm-ss.fffffffZ}'";
					break;
				case DbType.Double:
					edmType = TableStorageConstants.Edm.TYPE_DOUBLE;
					dataType = typeof(double);
					formatString = "{0}";
					break;
				case DbType.Guid:
					edmType = TableStorageConstants.Edm.TYPE_GUID;
					dataType = typeof(Guid);
					formatString = "'{0:D}'";
					break;
				case DbType.Int32:
					edmType = TableStorageConstants.Edm.TYPE_INT;
					dataType = typeof(int);
					formatString = "{0}";
					break;
				case DbType.Int64:
					edmType = TableStorageConstants.Edm.TYPE_LONG;
					dataType = typeof(long);
					formatString = "{0}";
					break;
				case DbType.String:
					edmType = TableStorageConstants.Edm.TYPE_STRING;
					dataType = typeof(string);
					formatString = "'{0}'";
					break;
				default:
					throw new NotSupportedException(string.Format(Resources.TypeNotSupported, parameter.DbType));
			}

			return new TableStorageField(value, edmType, parameter.ParameterName, isNull, dataType, formatString);
		}
	}
}