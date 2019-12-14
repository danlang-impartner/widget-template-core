using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Impartner.Microservice.Training.Models
{
	public class QuizAnswer
	{
		[Required]
		[BsonRepresentation(BsonType.ObjectId)]
		public string QuestionId { get; set; }
		[ReadOnly(true)]
		public bool IsCorrect { get; set; }
		public List<string> SelectedAnswers { get; set; } = new List<string>();
	}
}