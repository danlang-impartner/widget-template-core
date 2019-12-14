using System.Collections.Generic;
using System.Linq;
using System.Net;
using Impartner.Microservice.Common.Models.Responses;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Impartner.Microservice.Common.Exceptions
{
	public class InvalidJsonPatchException : ImpartnerException
	{
		#region Properties

		/// <summary>
		/// HTTP Status Code that the exception translates to.
		/// </summary>
		public override HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the exception using the model state errors that were produced in the attempt to apply a JSON patch.
		/// </summary>
		/// <param name="modelState">The model state to document in the error.</param>
		public InvalidJsonPatchException(ModelStateDictionary modelState)
			: base
			(
				"Invalid JSON Patch",
				new ApiError
				(
					nameof(InvalidJsonPatchException),
					"Invalid JSON Patch. Details: {details}",
					additionalData: new Dictionary<string, IEnumerable<string>>
					{
						{
							"details",
							modelState
								.Where(value => value.Value.ValidationState == ModelValidationState.Invalid)
								.SelectMany(value => value.Value.Errors.Select(error => error.ErrorMessage))
						}
					}
				)
			) { }

		#endregion
	}
}
