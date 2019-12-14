using System;
using System.Net;
using Impartner.Microservice.Common.Models.Responses;

namespace Impartner.Microservice.Common.Exceptions
{
	/// <summary>
	/// Base class for an exception that translates to an HTTP Response.
	/// </summary>
	public abstract class ImpartnerException : Exception
	{
		#region Properties

		/// <summary>
		/// The status code the exception that this exception translates to.
		/// </summary>
		public virtual HttpStatusCode StatusCode { get; protected set; } = HttpStatusCode.InternalServerError;

		/// <summary>
		/// Collection of errors associated with this exception.
		/// </summary>
		public ApiError[] Errors { get; set; }

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a general exception that takes a collection of errors to represent the exception.
		/// </summary>
		/// <param name="errors">Collection of errors associated with this exception.</param>
		protected ImpartnerException(params ApiError[] errors)
		{
			Errors = errors;
		}

		/// <summary>
		/// Constructs a general exception that takes a message and a collection of errors to represent the exception.
		/// </summary>
		/// <param name="message">Message describing the cause of the exception.</param>
		/// <param name="errors">Collection of errors associated with this exception.</param>
		protected ImpartnerException(string message, params ApiError[] errors) : base(message)
		{
			Errors = errors;
		}

		/// <summary>
		/// Constructs a general exception that wraps another exception, that takes a message and a collection of errors to represent the exception.
		/// </summary>
		/// <param name="message">Message describing the cause of the exception.</param>
		/// <param name="innerException">Exception that is being wrapped by this exception.</param>
		/// <param name="errors">Collection of errors associated with this exception.</param>
		protected ImpartnerException(string message, Exception innerException, params ApiError[] errors) : base(message, innerException)
		{
			Errors = errors;
		}

		#endregion
	}
}
