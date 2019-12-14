using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.Training.Models
{
	public class LessonContent
	{
		public int? BlobId { get; set; }
		public string Url { get; set; }
		[RegularExpression("^(.*)$")]
		public string Text { get; set; }
	}
}
