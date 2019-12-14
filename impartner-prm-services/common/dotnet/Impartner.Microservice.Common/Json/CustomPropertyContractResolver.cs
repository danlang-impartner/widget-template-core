using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Impartner.Microservice.Common.Json
{
	/// <summary>
	/// Collection of functions that can be used to modify the custom property contract.
	/// </summary>
	public static class CustomPropertyContractModifiers
	{
		#region Properties

		/// <summary>
		/// Marks the property as not writable if it is tagged with a Readonly attribute.
		/// </summary>
		public static CustomPropertyContractModifierFunction ReadOnly =>
			(ref JsonProperty jsonProperty, MemberInfo methodInfo, MemberSerialization methodSerialization) =>
			{
				var readonlyAttribute = methodInfo.GetCustomAttribute<ReadOnlyAttribute>();

				if (readonlyAttribute != null && readonlyAttribute.IsReadOnly)
				{
					jsonProperty.Writable = false;
				}
			};

		#endregion

		#region Public Methods

		/// <summary>
		/// Aggregator function that will combine <see cref="CustomPropertyContractModifierFunction"/> into a single delegate.
		/// </summary>
		/// <param name="functions"></param>
		/// <returns></returns>
		public static CustomPropertyContractModifierFunction CombineFunctions(
			params CustomPropertyContractModifierFunction[] functions
		) => functions.Aggregate(ReadOnly, (current, value) => current + value);

		#endregion
	}

	/// <summary>
	/// Contract resolver that overrides the <see cref="DefaultContractResolver.CreateProperty"/> function by adding the
	/// ability to customize the final results of the <see cref="JsonProperty"/>.
	/// </summary>
	public class CustomPropertyContractResolver : DefaultContractResolver
	{
		#region Fields

		/// <summary>
		/// Function that runs after a property is created during serialization but before it is returned to the object function.
		/// </summary>
		private readonly CustomPropertyContractModifierFunction _resolverFunction;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs the resolver using the provided delegate or an empty delegate if null.
		/// </summary>
		/// <param name="resolverFunction">The modifier function that is ran after a property has been created but before it is returned.</param>
		public CustomPropertyContractResolver(CustomPropertyContractModifierFunction resolverFunction)
		{
			_resolverFunction = resolverFunction ?? ((ref JsonProperty a, MemberInfo b, MemberSerialization c) => {});
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Creates a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.
		/// </summary>
		/// <param name="memberSerialization">The member's parent <see cref="T:Newtonsoft.Json.MemberSerialization" />.</param>
		/// <param name="member">The member to create a <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for.</param>
		/// <returns>A created <see cref="T:Newtonsoft.Json.Serialization.JsonProperty" /> for the given <see cref="T:System.Reflection.MemberInfo" />.</returns>
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			_resolverFunction(ref property, member, memberSerialization);

			return property;
		}

		#endregion
	}

	/// <summary>
	/// Function that allows updating a <see cref="JsonProperty"/> using details from the <see cref="MethodInfo"/> and <see cref="MemberSerialization"/>.
	/// </summary>
	/// <param name="jsonProperty">Property that has been created and is being updated based on information about the property.</param>
	/// <param name="memberInfo">Information about the member that is being serialized.</param>
	/// <param name="methodSerialization">Rule about how to serialize the property by default.</param>
	public delegate void CustomPropertyContractModifierFunction(ref JsonProperty jsonProperty, MemberInfo memberInfo, MemberSerialization methodSerialization);
}
