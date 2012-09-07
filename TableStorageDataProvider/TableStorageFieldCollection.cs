using System;
using System.Collections.ObjectModel;

namespace Magurany.Data.TableStorage
{
	internal sealed class TableStorageFieldCollection : KeyedCollection<string, TableStorageField>
	{
		public TableStorageFieldCollection() : base(StringComparer.OrdinalIgnoreCase) { }

		protected override string GetKeyForItem(TableStorageField item)
		{
			ThrowHelper.CheckArgumentNull(item, "item");

			return item.Name;
		}
	}
}