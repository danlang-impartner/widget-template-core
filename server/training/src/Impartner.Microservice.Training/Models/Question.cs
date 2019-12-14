using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Impartner.Microservice.Training.Models
{
	public class Question
	{
		public ObjectId Id { get; set; }
		public string Statement { get; set; }
		public List<string> PossibleAnswers { get; set; }
		public List<string> CorrectAnswers { get; set; }

		public Question()
		{
			Id = ObjectId.GenerateNewId();
		}
	}

	public class CertStatusQuestion
	{
		public ObjectId QuestionId { get; set; }
		public string Statement { get; set; }
		public List<string> PossibleAnswers { get; set; }
		[JsonIgnore]
		public List<string> CorrectAnswers { get; set; }
		[BsonIgnore]
		public int CorrectAnswerCount => CorrectAnswers.Count;

		public CertStatusQuestion() { }

		public CertStatusQuestion(Question question)
		{
			QuestionId = question.Id;
			Statement = question.Statement;
			PossibleAnswers = question.PossibleAnswers;
			CorrectAnswers = question.CorrectAnswers;
		}
	}
}
