using System;

namespace Magurany.Data.TableStorage
{
	/// <summary>
	/// When applied to a parameter, informs the static code analysis engine that the parameter
	/// is checked for null in the containing method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class ValidatedNotNullAttribute : Attribute { }
}