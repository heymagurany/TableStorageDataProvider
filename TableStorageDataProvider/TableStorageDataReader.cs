using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Xml;

namespace Magurany.Data.TableStorage
{
	public sealed class TableStorageDataReader : DbDataReader
	{
		private readonly int m_FieldCount;
		private readonly bool m_HasRows;
		private bool m_IsClosed;
		private readonly XmlReader m_Reader;
		private TableStorageFieldCollection m_Current;
		private TableStorageFieldCollection m_Next;

		internal TableStorageDataReader(Stream stream)
		{
			m_Current = new TableStorageFieldCollection();

			if(stream != null)
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.CloseInput = true;
				settings.IgnoreComments = true;
				settings.IgnoreWhitespace = true;

				m_Reader = XmlReader.Create(stream, settings);
				
				ReadNext();

				if(m_Next != null)
				{
					m_HasRows = true;
					m_FieldCount = m_Next.Count;
				}
			}
		}

		public override void Close()
		{
			if(m_Reader != null)
			{
				m_Reader.Close();
			}

			m_IsClosed = true;
		}

		private void EnsureOpen()
		{
			if(m_IsClosed)
			{
				throw new InvalidOperationException();
			}
		}

		public override DataTable GetSchemaTable()
		{
			EnsureOpen();

			throw new NotImplementedException();
		}

		public override bool NextResult()
		{
			return false;
		}

		public override bool Read()
		{
			if(m_Next == null)
			{
				return false;
			}

			m_Current = m_Next;

			ReadNext();

			return true;
		}

		private void ReadNext()
		{
			m_Next = null;

			if(m_Reader != null)
			{
				if(m_Reader.ReadToFollowing("entry", TableStorageConstants.Atom.NAMESPACE))
				{
					m_Reader.ReadStartElement("entry", TableStorageConstants.Atom.NAMESPACE);

					if(m_Reader.ReadToFollowing("properties", TableStorageConstants.Edm.NAMESPACE))
					{
						m_Reader.ReadStartElement("properties", TableStorageConstants.Edm.NAMESPACE);

						m_Next = new TableStorageFieldCollection();

						while(m_Reader.IsStartElement())
						{
							bool isEmpty = m_Reader.IsEmptyElement;
							string propertyName = m_Reader.LocalName;
							bool isNull;
							string edmType;

							if(m_Reader.MoveToAttribute("null", TableStorageConstants.Edm.NAMESPACE))
							{
								isNull = m_Reader.ReadContentAsBoolean();
							}
							else
							{
								isNull = false;
							}

							if(m_Reader.MoveToAttribute("type", TableStorageConstants.Edm.NAMESPACE))
							{
								edmType = m_Reader.ReadContentAsString();
							}
							else
							{
								edmType = TableStorageConstants.Edm.TYPE_STRING;
							}

							m_Reader.ReadStartElement(propertyName, TableStorageConstants.DataServices.NAMESPACE);

							TableStorageField field = ToObject(m_Reader, edmType, isNull, propertyName);

							m_Next.Add(field);

							if(!isEmpty)
							{
								m_Reader.ReadEndElement();
							}
						}
					}
				}
			}
		}

		public override int Depth
		{
			get { return 1; }
		}

		public override bool IsClosed
		{
			get { return m_IsClosed; }
		}

		public override int RecordsAffected
		{
			get { throw new NotSupportedException(); }
		}

		public override bool GetBoolean(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (bool)field.Value;
		}

		public override byte GetByte(int ordinal)
		{
			throw new NotSupportedException();
		}

		public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
		{
			TableStorageField field = m_Current[ordinal];
			Stream data = (Stream)field.Value;

			data.Seek(dataOffset, SeekOrigin.Begin);

			return data.Read(buffer, bufferOffset, length);
		}

		public override char GetChar(int ordinal)
		{
			throw new NotSupportedException();
		}

		public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
		{
			throw new NotSupportedException();
		}

		public override Guid GetGuid(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (Guid)field.Value;
		}

		public override short GetInt16(int ordinal)
		{
			throw new NotSupportedException();
		}

		public override int GetInt32(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (int)field.Value;
		}

		public override long GetInt64(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (long)field.Value;
		}

		public override DateTime GetDateTime(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (DateTime)field.Value;
		}

		public override string GetString(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (string)field.Value;
		}

		public override object GetValue(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return field.Value;
		}

		public override int GetValues(object[] values)
		{
			ThrowHelper.CheckArgumentNull(values, "values");

			for(int index = 0; index < values.Length && index < m_Current.Count; index++)
			{
				values[index] = m_Current[index].Value;
			}

			return m_Current.Count;
		}

		public override bool IsDBNull(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return field.IsNull;
		}

		public override int FieldCount
		{
			get { return m_FieldCount; }
		}

		public override object this[int ordinal]
		{
			get
			{
				TableStorageField field = m_Current[ordinal];

				return field.Value;
			}
		}

		public override object this[string name]
		{
			get
			{
				TableStorageField field = m_Current[name];

				return field.Value;
			}
		}

		public override bool HasRows
		{
			get { return m_HasRows; }
		}

		public override decimal GetDecimal(int ordinal)
		{
			throw new NotSupportedException();
		}

		public override double GetDouble(int ordinal)
		{
			TableStorageField field = m_Current[ordinal];

			return (double)field.Value;
		}

		public override float GetFloat(int ordinal)
		{
			throw new NotSupportedException();
		}

		public override string GetName(int ordinal)
		{
			if((m_Next != null) && (ordinal >= 0) && (ordinal < m_Next.Count))
			{
				return m_Next[ordinal].Name;
			}

			throw ThrowHelper.ThrowObjectNotFound(ordinal, "ordinal", "column at ordinal");
		}

		public override int GetOrdinal(string name)
		{
			if((m_Next != null) && m_Next.Contains(name))
			{
				TableStorageField field = m_Next[name];
				
				return m_Next.IndexOf(field);
			}

			throw ThrowHelper.ThrowObjectNotFound(name, "name", "column with name");
		}

		public override string GetDataTypeName(int ordinal)
		{
			if((m_Next != null) && (ordinal >= 0) && (ordinal < m_Next.Count))
			{
				return m_Current[ordinal].EdmType;
			}

			throw ThrowHelper.ThrowObjectNotFound(ordinal, "ordinal", "column at ordinal");
		}

		public override Type GetFieldType(int ordinal)
		{
			if((m_Next != null) && (ordinal >= 0) && (ordinal < m_Next.Count))
			{
				return m_Next[ordinal].DataType;
			}

			throw ThrowHelper.ThrowObjectNotFound(ordinal, "ordinal", "column at ordinal");
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		private static TableStorageField ToObject(XmlReader reader, string edmType, bool isNull, string propertyName)
		{
			object value;
			Type dataType;
			string formatString;

			if(edmType == TableStorageConstants.Edm.TYPE_BINARY)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					using(Stream stream = new MemoryStream())
					{
						const int size = 256;
						byte[] buffer = new byte[size];
						int count;

						while((count = reader.ReadContentAsBase64(buffer, 0, size)) > 0)
						{
							stream.Write(buffer, 0, count);
						}

						stream.Seek(0, SeekOrigin.Begin);

						value = stream;
					}
				}

				dataType = typeof(byte[]);
				formatString = "{0}";
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_BOOLEAN)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsBoolean();
				}

				dataType = typeof(bool);
				formatString = "{0}";
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_DATETIME)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsDateTime();
				}

				dataType = typeof(DateTime);
				formatString = TableStorageConstants.Edm.DATE_TIME_FORMAT;
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_DOUBLE)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsDouble();
				}

				dataType = typeof(double);
				formatString = "{0}";
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_GUID)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = Guid.Parse(reader.ReadContentAsString());
				}

				dataType = typeof(Guid);
				formatString = "{0:D}";
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_INT)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsInt();
				}

				dataType = typeof(int);
				formatString = "{0}";
			}
			else if(edmType == TableStorageConstants.Edm.TYPE_LONG)
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsLong();
				}

				dataType = typeof(long);
				formatString = "{0}";
			}
			else if(StringComparer.OrdinalIgnoreCase.Equals(TableStorageConstants.Edm.TYPE_STRING, edmType))
			{
				if(isNull)
				{
					value = DBNull.Value;
				}
				else
				{
					value = reader.ReadContentAsString();
				}

				dataType = typeof(string);
				formatString = "{0}";
			}
			else
			{
				throw new TableStorageException(string.Format("The type, '{0},' is not supported.", edmType));
			}

			return new TableStorageField(value, edmType, propertyName, isNull, dataType, formatString);
		}
	}
}