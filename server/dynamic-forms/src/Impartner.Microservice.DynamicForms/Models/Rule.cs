using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.DynamicForms.Models
{
	public class Rule
	{
		[Required]
		public ItemState State { get; set; }

		[Required]
		public RuleConditions Conditions { get; set; }
	}
}
