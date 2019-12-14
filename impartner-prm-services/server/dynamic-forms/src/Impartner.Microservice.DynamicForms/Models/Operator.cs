using System.Runtime.Serialization;

namespace Impartner.Microservice.DynamicForms.Models
{
	public enum Operator
	{
		[EnumMember( Value = "equal" )]
		Equal,
		[EnumMember( Value = "notEqual" )]
		NotEqual,
		[EnumMember( Value = "lessThan" )]
		LessThan,
		[EnumMember( Value = "lessThanInclusive" )]
		LessThanInclusive,
		[EnumMember( Value = "greaterThan" )]
		GreaterThan,
		[EnumMember( Value = "greaterThanInclusive" )]
		GreaterThanInclusive,
		[EnumMember( Value = "in" )]
		In,
		[EnumMember( Value = "notIn" )]
		NotIn,
		[EnumMember( Value = "contains" )]
		Contains,
		[EnumMember( Value = "doesNotContain" )]
		DoesNotContain
	}
}