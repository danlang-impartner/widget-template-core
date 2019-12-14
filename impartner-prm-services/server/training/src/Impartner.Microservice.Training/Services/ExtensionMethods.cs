using Impartner.Microservice.Common.Models;
using Impartner.Microservice.Training.Models;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Impartner.Microservice.Training.Services
{
	public static class ExtensionMethods
	{
		public static CourseCompletion ToCourseCompletion(this CourseStatus status)
		{
			var quizAttempts = 0;
			status.Quizzes.ForEach(q => quizAttempts += q.QuizStatuses.Count);

			return new CourseCompletion
			{
				CourseVersion = status.CourseVersion,
				ProgramId = status.TenantId,
				CertificationId = status.CertificationId,
				CourseId = status.CourseId,
				NumberOfAttempts = quizAttempts,
				CompletedAt = status.CompletedAt ?? DateTime.UtcNow,
				CreatedBy = status.CreatedBy,
				Name = status.Name,
				Description = status.Description
			};
		}

		public static CertificationCompletion ToCertificationCompletion(this CertificationStatus status)
		{
			var now = DateTime.UtcNow;
			var completion = new CertificationCompletion
			{
				CertificationVersion = status.CertificationVersion,
				ProgramId = status.TenantId,
				Id = status.CertificationId,
				Name = status.Name,
				Description = status.Description,
				CreatedBy = status.CreatedBy,
				CompletedAt = now
			};
			if (status.Expiration > 0)
				completion.Expiration = now.AddMonths(status.Expiration);
			return completion;
		}

		public static Stream ToStream(this object data)
		{
			var json = JsonConvert.SerializeObject(data);
			var bytes = Encoding.ASCII.GetBytes(json);
			return new MemoryStream(bytes);
		}

		public class CertificationCompletion
		{
			public int CertificationVersion { get; set; }
			public ObjectId Id { get; set; }
			public int ProgramId { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public User CreatedBy { get; set; }
			public DateTime CompletedAt { get; set; }
			public DateTime? Expiration { get; set; }
		}

		public class CourseCompletion
		{
			public int CourseVersion { get; set; }
			public int ProgramId { get; set; }
			public ObjectId CertificationId { get; set; }
			public ObjectId CourseId { get; set; }
			public int NumberOfAttempts { get; set; }
			public DateTime CompletedAt { get; set; }
			public User CreatedBy { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
		}
	}
}
