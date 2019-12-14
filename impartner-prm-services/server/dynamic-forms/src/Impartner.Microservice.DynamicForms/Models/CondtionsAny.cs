using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
namespace Impartner.Microservice.DynamicForms.Models
{
	public class RuleConditions : IValidatableObject
	{
		/// <summary>
		/// Essentially a collection of OR rules, where any of them must be true..
		/// </summary>
		public List<Dictionary<string, List<Condition>>> Any { get; set; } = new List<Dictionary<string, List<Condition>>>();

		/// <summary>
		/// Essentially a collection of AND rules, where all must be true.
		/// </summary>
		public List<Dictionary<string, List<Condition>>> All { get; set; } = new List<Dictionary<string, List<Condition>>>();

		/// <summary>Determines whether the specified object is valid.</summary>
		/// <param name="validationContext">The validation context.</param>
		/// <returns>A collection that holds failed-validation information.</returns>
		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (Any.Any() && All.Any() || !All.Any() && !Any.Any())
			{
				return new []
				{
					Any.Any() && All.Any() ?
						new ValidationResult($"Both {nameof(Any)} and {nameof(All)} properties have items, which is invalid. One or the other must have items provided.") :
						new ValidationResult($"Neither {nameof(Any)} and {nameof(All)} properties have items, which is invalid. One or the other must have items provided."),
				};
			}

			return Enumerable.Empty<ValidationResult>();
		}
	}
}
