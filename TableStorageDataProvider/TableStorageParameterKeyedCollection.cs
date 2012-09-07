using System;
using System.Collections.ObjectModel;

namespace Magurany.Data.TableStorage
{
	internal sealed class TableStorageParameterKeyedCollection : KeyedCollection<string, TableStorageParameter>
	{
		internal TableStorageParameterKeyedCollection() : base(StringComparer.OrdinalIgnoreCase) { }

		protected override string GetKeyForItem(TableStorageParameter item)
		{
			ThrowHelper.CheckArgumentNull(item, "item");

			return item.ParameterName;
		}
	}
}