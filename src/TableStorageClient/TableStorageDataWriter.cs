using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Magurany.Data.TableStorageClient
{
	internal sealed class TableStorageDataWriter : IDisposable
	{
		private readonly XmlWriter m_Writer;

		internal TableStorageDataWriter(Stream stream)
		{
			XmlWriterSettings settings = new XmlWriterSettings();
			settings.CloseOutput = false;
			settings.Encoding = Encoding.UTF8;

			TextWriter traceWriter = new StreamWriter(stream);

			m_Writer = XmlWriter.Create(traceWriter, settings);
		}

		public void Close()
		{
			m_Writer.Close();
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}

		private void Dispose(bool disposing)
		{
			if(disposing)
			{
				Close();
			}
		}

		public void Flush()
		{
			m_Writer.Flush();
		}

		public void WriteEndEntry()
		{
			m_Writer.WriteEndElement();
			m_Writer.WriteEndElement();
			m_Writer.WriteEndElement();
		}

		public void WriteField(TableStorageField parameter)
		{
			m_Writer.WriteStartElement(parameter.Name, TableStorageConstants.DataServices.NAMESPACE);
			m_Writer.WriteAttributeString("type", TableStorageConstants.Edm.NAMESPACE, parameter.EdmType);

			object value = parameter.Value;

			if((value == null) || DBNull.Value.Equals(value))
			{
				m_Writer.WriteAttributeString("null", TableStorageConstants.Edm.NAMESPACE, "true");
			}
			else
			{
				m_Writer.WriteValue(value);
			}

			m_Writer.WriteEndElement();
		}

		public void WriteStartEntry(string entryID)
		{
			m_Writer.WriteStartDocument(true);
			m_Writer.WriteStartElement("entry", TableStorageConstants.Atom.NAMESPACE);
			m_Writer.WriteElementString("title", TableStorageConstants.Atom.NAMESPACE, null);
			m_Writer.WriteStartElement("updated", TableStorageConstants.Atom.NAMESPACE);
			m_Writer.WriteString(string.Format(TableStorageConstants.Edm.DATE_TIME_FORMAT, DateTime.UtcNow));
			m_Writer.WriteEndElement();
			m_Writer.WriteStartElement("author", TableStorageConstants.Atom.NAMESPACE);
			m_Writer.WriteElementString("name", TableStorageConstants.Atom.NAMESPACE, null);
			m_Writer.WriteEndElement();
			m_Writer.WriteElementString("id", TableStorageConstants.Atom.NAMESPACE, entryID);
			m_Writer.WriteStartElement("content", TableStorageConstants.Atom.NAMESPACE);
			m_Writer.WriteAttributeString("type", "application/xml");
			m_Writer.WriteStartElement("properties", TableStorageConstants.Edm.NAMESPACE);
		}
	}
}