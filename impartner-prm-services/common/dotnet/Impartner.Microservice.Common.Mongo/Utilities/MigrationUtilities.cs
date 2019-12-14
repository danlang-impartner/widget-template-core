using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mongo.Migration.Documents;
using MongoDB.Bson.Serialization.Attributes;

namespace Impartner.Microservice.Common.Mongo.Utilities
{
	/// <summary>
	/// Utilities used to provide functionality used for migrations, such as getting expected and consistent names for properties.
	/// </summary>
	public static class MigrationUtilities
	{
		/// <summary>
		/// Gets the name of the property expressed in the given expression, checking first the BsonElement declaration, and then a camel case version of the property.
		/// </summary>
		/// <typeparam name="TDocument">Document that will be saved in the database or at least converted to BSON.</typeparam>
		/// <param name="objectMember">The member to retrieve the name of.</param>
		/// <param name="useDiscriminator">
		/// Whether or not to use a discriminator as part of the value name. The class name is used as the discriminator. This should only be used to handle name conflicts between versions.
		/// </param>
		/// <returns>The name of the property provided by the expression.</returns>
		public static string GetNameOf<TDocument>(Expression<Func<TDocument, object>> objectMember, bool useDiscriminator = false)
			where TDocument : class, IDocument
		{
			// NOTE - Depending on the property/field and it's type, it may be a MemberExpression(Reference type) or a UnaryExpression(primitive type/struct).
			// The following code retrieves the member info for either case. If it is anything else, throw an exception
			var memberInfo = (objectMember.Body as MemberExpression)?.Member;
			if (memberInfo == null && objectMember.Body is UnaryExpression unaryExpression)
			{
				memberInfo = (unaryExpression.Operand as MemberExpression)?.Member;
			}

			if (memberInfo == null)
			{
				throw new ArgumentException("Member information was not a property or field, or could not be retrieved", nameof(objectMember));
			}
			
			var bsonNameAttributes = memberInfo.GetCustomAttributes<BsonElementAttribute>();

			// Order of operations here is if BsonElement name exists, use that name, otherwise use the name of the property.
			var name = bsonNameAttributes.FirstOrDefault(value => value.ElementName != null)?.ElementName ?? memberInfo.Name;

			if (memberInfo.DeclaringType == null && useDiscriminator)
			{
				throw new ArgumentException($"Discriminator cannot be used because {nameof(memberInfo.DeclaringType)} is null", nameof(useDiscriminator));
			}

			if (useDiscriminator)
			{
				name += $"+{memberInfo.DeclaringType.Name}";
			}

			// Convert name to camel case. TODO - Not safe.
			return char.ToLowerInvariant(name[0]) + name.Substring(1);
		}
	}
}
