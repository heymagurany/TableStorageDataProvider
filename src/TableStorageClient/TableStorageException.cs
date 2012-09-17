using System;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Xml;

namespace Magurany.Data.TableStorageClient
{
	[Serializable]
	public sealed class TableStorageException : DbException
	{
		private readonly string m_Code;

		public TableStorageException() { }

		public TableStorageException(string message) : base(message) { }

		private TableStorageException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			m_Code = info.GetString("Code");
		}

		public TableStorageException(string message, Exception innerException) : base(message, innerException) { }

		private TableStorageException(string code, string message, Exception innerException) : this(message, innerException)
		{
			m_Code = code;
		}

		public string Code
		{
			get { return m_Code; }
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Code", m_Code);
		}

		public static TableStorageException Parse(WebException innerException)
		{
			ThrowHelper.CheckArgumentNull(innerException, "innerException");

			HttpWebResponse response = (HttpWebResponse)innerException.Response;

			if(response == null)
			{
				return new TableStorageException(innerException.Message, innerException);
			}

			Stream stream = response.GetResponseStream();

			Debug.Assert(stream != null);

			if(stream.CanSeek)
			{
				stream.Seek(0, SeekOrigin.Begin);
			}

			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreComments = true;
			settings.IgnoreWhitespace = true;

			XmlReader reader = XmlReader.Create(stream, settings);
			reader.ReadStartElement("error");
			reader.ReadStartElement("code");
			string tableStorageErrorCode = reader.ReadContentAsString();
			reader.ReadEndElement();
			reader.ReadStartElement("message");
			string message = reader.ReadContentAsString();
			reader.ReadEndElement();

			TableStorageException exception = new TableStorageException(tableStorageErrorCode, message, innerException);

			while(reader.IsStartElement())
			{
				string name = reader.LocalName;
				bool isEmpty = reader.IsEmptyElement;

				reader.ReadStartElement(name);

				if(!isEmpty)
				{
					if(!exception.Data.Contains(name))
					{
						string value = reader.ReadContentAsString();

						exception.Data.Add(name, value);
					}

					reader.ReadEndElement();
				}
			}

			reader.ReadEndElement();

			return exception;
		}
	}
}