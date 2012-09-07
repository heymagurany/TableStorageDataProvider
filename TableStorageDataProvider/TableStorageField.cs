using System;

namespace Magurany.Data.TableStorage
{
	internal sealed class TableStorageField
	{
		private readonly object m_Value;
		private readonly string m_EdmType;
		private readonly string m_Name;
		private readonly bool m_IsNull;
		private readonly Type m_DataType;
		private readonly string m_FormatString;

		public TableStorageField(object value, string edmType, string name, bool isNull, Type dataType, string formatString)
		{
			m_Value = value;
			m_FormatString = formatString;
			m_DataType = dataType;
			m_IsNull = isNull;
			m_Name = name;
			m_EdmType = edmType;
		}

		public string FormatString
		{
			get { return m_FormatString; }
		}

		public Type DataType
		{
			get { return m_DataType; }
		}

		public bool IsNull
		{
			get { return m_IsNull; }
		}

		public string Name
		{
			get { return m_Name; }
		}

		public string EdmType
		{
			get { return m_EdmType; }
		}

		public object Value
		{
			get { return m_Value; }
		}
	}
}
