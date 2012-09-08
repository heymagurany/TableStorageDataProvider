using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Magurany.Data.TableStorageClient.Properties;

namespace Magurany.Data.TableStorageClient
{
	/// <summary>
	/// Contains methods that throw exceptions with consistant messages.
	/// </summary>
	internal static class ThrowHelper
	{
		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if <parameterref name="parameter"/> is a null reference.
		/// </summary>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static void CheckArgumentNull([ValidatedNotNull]object parameter, string parameterName)
		{
			if(parameter == null)
			{
				throw ThrowArgumentNull(parameterName);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if <parameterref name="parameter"/> is a null reference or
		/// an <see cref="ArgumentException"/> if <parameterref name="parameter"/> only contins white space characters.
		/// </summary>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static void CheckArgumentNullOrEmpty([ValidatedNotNull]string parameter, string parameterName)
		{
			CheckArgumentNull(parameter, parameterName);

			if(parameter.Trim().Length == 0)
			{
				throw ThrowArgumentEmpty(parameterName);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException"/> if <parameterref name="parameter"/> is a null reference or
		/// an <see cref="ArgumentException"/> if <parameterref name="parameter"/> only contins white space characters.
		/// </summary>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static void CheckArgumentNullOrEmpty<T>([ValidatedNotNull]IEnumerable<T> parameter, string parameterName)
		{
			CheckArgumentNull(parameter, parameterName);

			if(parameter.Count() == 0)
			{
				throw ThrowArgumentEmpty(parameterName);
			}
		}

		/// <summary>
		/// Returns an <see cref="ArgumentNullException"/>.
		/// </summary>
		/// <returns> An instance of <see cref="ArgumentNullException"/> containing a message describing the exception. </returns>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static ArgumentNullException ThrowArgumentNull(string parameterName)
		{
			return new ArgumentNullException(parameterName, Resources.ThrowArgumentNull);
		}

		/// <summary>
		/// Returns an <see cref="ArgumentException"/>.
		/// </summary>
		/// <returns> An instance of <see cref="ArgumentException"/> containing a message describing the exception. </returns>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static ArgumentException ThrowArgumentEmpty(string parameterName)
		{
			return new ArgumentException(string.Format(Resources.ThrowArgumentEmpty, parameterName), parameterName);
		}

		/// <summary>
		/// Returns an <see cref="ArgumentOutOfRangeException"/>.
		/// </summary>
		/// <parameter name="parameter"> The value that was not found. </parameter>
		/// <parameter name="parameterName"> The name of the argument that contains <parameterref name="parameter"/>. </parameter>
		/// <parameter name="objectName"> The name of the object that wasn't found. </parameter>
		/// <returns> An instance of <see cref="ArgumentOutOfRangeException"/> containing a message describing the exception. </returns>
		/// <example>
		/// The following example throws an exception with the message, 'The user, 'jdoe,' was not found.'
		/// <code>
		/// ThrowObjectNotFound("jdoe", "userName, "user");
		/// </code>
		/// </example>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static ArgumentOutOfRangeException ThrowObjectNotFound(object parameter, string parameterName, string objectName)
		{
			return new ArgumentOutOfRangeException(parameterName, parameter, string.Format(Resources.ThrowObjectNotFound, objectName, parameter));
		}

		/// <summary>
		/// Returns an <see cref="ArgumentException"/>.
		/// </summary>
		/// <parameter name="parameter"> The value that already exists. </parameter>
		/// <parameter name="parameterName"> The name of the argument that contains <parameterref name="parameter"/>. </parameter>
		/// <parameter name="objectName"> The name of the object that already exists. </parameter>
		/// <returns> An instance of <see cref="ArgumentException"/> containing a message describing the exception. </returns>
		/// <example>
		/// The following example throws an exception with the message, "The <parameterref name="objectName"/>, '<parameterref name="parameter"/>,' already exists."
		/// <code>
		/// ThrowObjectExists("jdoe", "userName, "user");
		/// </code>
		/// </example>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static ArgumentException ThrowObjectExists(object parameter, string parameterName, string objectName)
		{
			return new ArgumentException(string.Format(Resources.ThrowObjectExists, objectName, parameter), parameterName);
		}

		/// <summary>
		/// Returns an <see cref="ArgumentException"/>.
		/// </summary>
		/// <parameter name="parameterName"> The name of the offending argument. </parameter>
		/// <parameter name="expectedType"> The type that <parameterref name="parameterName"/> was expected to be assignable to. </parameter>
		/// <returns> An instance of <see cref="ArgumentException"/> containing a message describing the exception. </returns>
		/// <example>
		/// The following example throws an exception with the message, "The argument, '<parameterref name="parameterName"/>,' must be of type '<parameterref name="expectedType"/>.'"
		/// <code>
		/// throw ThrowArgumentWrongType&lt;int&gt;("obj", typeof(MyObject));
		/// </code>
		/// </example>
		[DebuggerNonUserCode]
		[DebuggerStepThrough]
		public static ArgumentException ThrowArgumentWrongType(string parameterName, Type expectedType)
		{
			return new ArgumentException(string.Format(Resources.ThrowArgumentWrongType, parameterName, expectedType), parameterName);
		}
	}
}