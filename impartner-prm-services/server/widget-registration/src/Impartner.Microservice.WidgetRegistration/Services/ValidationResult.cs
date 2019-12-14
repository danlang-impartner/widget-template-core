using System.Collections.Generic;

namespace Impartner.Microservice.WidgetRegistration.Services
{
	public class ValidationResult
	{
		public Dictionary<string, List<string>> ErrorDetails { get; }
		public bool IsValid { get; }

		public ValidationResult()
		{
			IsValid = true;
		}

		public ValidationResult(Dictionary<string, List<string>> errorDetails)
		{
			IsValid = false;
			ErrorDetails = errorDetails;
		}
	}
}
