using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;

namespace Impartner.Microservice.Training.Models
{
	public class QuizState
	{
		public QuizStatus Status { get; set; }
		[ReadOnly(true)]
		public int CorrectQuestions { get; set; }
		[ReadOnly(true)]
		public bool HasPassed { get; set; }
		[JsonIgnore]
		public bool IsComplete { get; set; }
		public List<QuizAnswer> Answers { get; set; } = new List<QuizAnswer>();
	}

	public enum QuizStatus
	{
		NotStarted,
		InProcess,
		Completed,
		Restarted
	}
}
