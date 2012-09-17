using System.Data;
using System.Data.Common;

namespace Magurany.Data.TableStorageClient
{
	/// <summary>
	/// Contains helper methods for creating <see cref="DbCommand"/> objects.
	/// </summary>
	public static class DbConnectionExtensions
	{
		/// <summary>
		/// Creates an instance of a <see cref="DbCommand"/> class with the specified command text.
		/// </summary>
		/// <param name="connection">The <see cref="DbConnection"/> instance to create the command for.</param>
		/// <param name="commandText">A <see cref="string"/> containting the command text.</param>
		/// <returns>A <see cref="DbCommand"/> instance with the command text set to <paramref name="commandText"/>.</returns>
		public static DbCommand CreateCommand(this DbConnection connection, string commandText)
		{
			DbCommand command = connection.CreateCommand();
			command.CommandText = commandText;
			command.CommandType = CommandType.Text;

			return command;
		}
	}
}