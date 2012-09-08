using System;
using System.Collections;
using System.Data.Common;

namespace Magurany.Data.TableStorageClient
{
	public sealed class TableStorageParameterCollection : DbParameterCollection
	{
		private readonly TableStorageParameterKeyedCollection m_Parameters;

		public TableStorageParameterCollection()
		{
			m_Parameters = new TableStorageParameterKeyedCollection();
		}

		public override int Count
		{
			get { return  m_Parameters.Count; }
		}

		public override object SyncRoot
		{
			get { return ((ICollection)m_Parameters).SyncRoot; }
		}

		public override bool IsFixedSize
		{
			get { return ((IList)m_Parameters).IsFixedSize; }
		}

		public override bool IsReadOnly
		{
			get { return ((IList)m_Parameters).IsReadOnly; }
		}

		public override bool IsSynchronized
		{
			get { return ((ICollection)m_Parameters).IsSynchronized; }
		}

		public override int Add(object value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);
			int index = Count;

			m_Parameters.Insert(index, parameter);

			return index;
		}

		public override void AddRange(Array values)
		{
			TableStorageParameter[] parameters = ConvertToArray(values);

			foreach(TableStorageParameter parameter in parameters)
			{
				m_Parameters.Add(parameter);
			}
		}

		public override void Clear()
		{
			m_Parameters.Clear();
		}

		public override bool Contains(object value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);
			
			return m_Parameters.Contains(parameter);
		}

		public override bool Contains(string value)
		{
			return m_Parameters.Contains(value);
		}

		private static TableStorageParameter[] ConvertToArray(Array array)
		{
			TableStorageParameter[] parameters = array as TableStorageParameter[];

			if(parameters == null)
			{
				throw ThrowHelper.ThrowArgumentWrongType("value", typeof(TableStorageParameter));
			}
			return parameters;
		}

		private static TableStorageParameter ConvertToParameter(object value)
		{
			TableStorageParameter parameter = value as TableStorageParameter;

			if(parameter == null)
			{
				throw ThrowHelper.ThrowArgumentWrongType("value", typeof(TableStorageParameter));
			}

			return parameter;
		}

		public override void CopyTo(Array array, int index)
		{
			TableStorageParameter[] parameters = ConvertToArray(array);

			m_Parameters.CopyTo(parameters, index);
		}

		public override IEnumerator GetEnumerator()
		{
			return m_Parameters.GetEnumerator();
		}

		protected override DbParameter GetParameter(int index)
		{
			return m_Parameters[index];
		}

		protected override DbParameter GetParameter(string parameterName)
		{
			return m_Parameters[parameterName];
		}

		public override int IndexOf(object value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);

			return m_Parameters.IndexOf(parameter);
		}

		public override int IndexOf(string parameterName)
		{
			TableStorageParameter parameter = m_Parameters[parameterName];
			
			return m_Parameters.IndexOf(parameter);
		}

		public override void Insert(int index, object value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);

			m_Parameters.Insert(index, parameter);
		}

		public override void Remove(object value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);
			
			m_Parameters.Remove(parameter);
		}

		public override void RemoveAt(int index)
		{
			m_Parameters.RemoveAt(index);
		}

		public override void RemoveAt(string parameterName)
		{
			m_Parameters.Remove(parameterName);
		}

		protected override void SetParameter(int index, DbParameter value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);

			m_Parameters.Insert(index, parameter);
		}

		protected override void SetParameter(string parameterName, DbParameter value)
		{
			TableStorageParameter parameter = ConvertToParameter(value);

			m_Parameters.Add(parameter);
		}
	}
}