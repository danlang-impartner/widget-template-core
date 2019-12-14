using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

// TODO - Upgrade to TenantDocument.V1
using TenantDocument = Impartner.Microservice.Common.Mongo.Models.TenantDocument.V0;

namespace Impartner.Microservice.Training.Models
{
	public abstract class TrainingBase : TenantDocument
	{
		[Required]
		[MaxLength(90)]
		public string Name { get; set; }
		[Required]
		public bool IsActive { get; set; }

		protected TrainingBase()
		{
			Id = ObjectId.GenerateNewId();
		}
	}
}
