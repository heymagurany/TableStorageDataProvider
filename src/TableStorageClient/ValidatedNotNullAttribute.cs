using System;

namespace Magurany.Data.TableStorageClient
{
	/// <summary>
	/// When applied to a parameter, informs the static code analysis engine that the parameter
	/// is checked for null in the containing method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	internal sealed class ValidatedNotNullAttribute : Attribute { }
}