using System;
using Impartner.Microservice.Common.Exceptions;
using Impartner.Microservice.Common.Json;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Impartner.Microservice.Common.Extensions
{
	public static class ControllerBaseExtensions
	{
		#region Public Methods

		/// <summary>
		/// Tries to apply the provided patch operations against the given model. If the patch operations succeed return true; Otherwise return false.
		/// </summary>
		/// <typeparam name="TModel">Type of the model being patched.</typeparam>
		/// <param name="controller">Controller that is performing the patch.</param>
		/// <param name="patchDocument">Collection of operations to apply to the model using JSON Patch.</param>
		/// <param name="objectToApplyTo">Model to apply operations to.</param>
		/// <param name="prefix">String value that can be used to decorate the property information that is recorded during validation errors.</param>
		/// <returns>If the patch operations succeed return true; Otherwise return false.</returns>
		public static bool TryApplyPatch<TModel>
		(
			this ControllerBase controller,
			JsonPatchDocument<TModel> patchDocument,
			TModel objectToApplyTo,
			string prefix = null
		) where TModel : class
		{
			try
			{
				ApplyPatch(controller, patchDocument, objectToApplyTo, prefix);
				return true;
			}
			catch (InvalidJsonPatchException)
			{
				return false;
			}
		}

		/// <summary>
		/// Applies the provided patch operations against the given model.
		/// </summary>
		/// <typeparam name="TModel">Type of the model being patched.</typeparam>
		/// <param name="controller">Controller that is performing the patch.</param>
		/// <param name="patchDocument">Collection of operations to apply to the model using JSON Patch.</param>
		/// <param name="objectToApplyTo">Model to apply operations to.</param>
		/// <param name="prefix">String value that can be used to decorate the property information that is recorded during validation errors.</param>
		/// <exception cref="InvalidJsonPatchException">Exception thrown when an operation can not be applied to the model.</exception>
		public static void ApplyPatch<TModel>
		(
			this ControllerBase controller,
			JsonPatchDocument<TModel> patchDocument,
			TModel objectToApplyTo,
			string prefix = null
		) where TModel : class
		{
			if (controller?.ModelState == null)
			{
				throw new ArgumentNullException(nameof(controller.ModelState), "");
			}

			if (controller.ObjectValidator == null)
			{
				throw new ArgumentNullException(nameof(controller.ObjectValidator));
			}

			if (patchDocument == null)
			{
				throw new ArgumentNullException(nameof(patchDocument));
			}

			if (objectToApplyTo == null)
			{
				throw new ArgumentNullException(nameof(objectToApplyTo));
			}

			patchDocument.ContractResolver = new CustomPropertyContractResolver(CustomPropertyContractModifiers.ReadOnly);
			patchDocument.ApplyTo(objectToApplyTo, error => ApplyErrorToModelState(error, controller.ModelState, prefix));

			if (!controller.TryValidateModel(objectToApplyTo))
			{
				throw new InvalidJsonPatchException(controller.ModelState);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Updates the model state with errors that were given by the patch operation.
		/// </summary>
		/// <param name="error">The error returned by the patch operation.</param>
		/// <param name="modelState">The model stat where validation issues can be added and tracked by property.</param>
		/// <param name="prefix">Prefix to use for the property name in the model state.</param>
		private static void ApplyErrorToModelState(JsonPatchError error, ModelStateDictionary modelState, string prefix)
		{
			var name = error.AffectedObject.GetType().Name;
			modelState.TryAddModelError
			(
				string.IsNullOrEmpty(prefix) ? name : $"{prefix}.{name}",
				$"Op: {error.Operation.OperationType}, Path: {error.Operation.path}, Value: {error.Operation.value}, Error: {error.ErrorMessage}"
			);
		}

		#endregion
	}
}
