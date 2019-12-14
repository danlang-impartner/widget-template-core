using System.Text.RegularExpressions;

namespace Impartner.Microservice.Common.Utilities
{
	/// <summary>
	/// Common utilities for interacting with strings.
	/// </summary>
	public static class StringUtilities
	{
		#region Public Methods

		/// <summary>
		/// Apply the truncate regex to a provided string.
		/// </summary>
		/// <param name="value">The value to truncate.</param>
		/// <param name="truncateRegex">The regex to apply to the string.</param>
		/// <returns>The truncated version of the string.</returns>
		public static string ApplyTruncateRegex(string value, Regex truncateRegex) => truncateRegex.Replace(value, "$1", 1);

		/// <summary>
		/// Creates a truncating regex that can be used to apply truncation on a string.
		/// </summary>
		/// <param name="min">Minimum required characters.</param>
		/// <param name="max">Maximum length of characters before truncating.</param>
		/// <returns>A regex that can perform the truncation.</returns>
		public static Regex CreateTruncateRegex(int min = 0, int max = int.MaxValue) => new Regex($@"^(.{{{min},{max - 1}}})(.*)$");

		#endregion
	}
}
