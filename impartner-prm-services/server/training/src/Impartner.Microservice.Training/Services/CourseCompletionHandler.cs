using Impartner.Microservice.Common.Mongo.Repositories;
using Impartner.Microservice.Training.Controllers;
using Impartner.Microservice.Training.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Impartner.Microservice.Training.Services
{
	public interface ICourseCompletionHandler
	{
		Task<bool> CompleteCourse(CertificationStatus certStatus, CourseStatus courseStatus);
	}

	public class CourseCompletionHandler : ICourseCompletionHandler
	{
		private readonly IMongoRepository _repository;
		private readonly IMessenger _messenger;
		public const string CourseCompletionName = "CourseCompletion";
		public const string CertificationCompletionName = "CertificationCompletion";

		public CourseCompletionHandler(IMongoRepository repository, IMessenger messenger)
		{
			_repository = repository;
			_messenger = messenger;
		}

		public async Task<bool> CompleteCourse(CertificationStatus certStatus, CourseStatus courseStatus)
		{
			await _messenger.SendMessage(CourseCompletionName, courseStatus.ToCourseCompletion());
			return await CheckForCertificationCompletion(certStatus);
		}

		private async Task<bool> CheckForCertificationCompletion(CertificationStatus certStatus)
		{
			if (certStatus.Courses.Any(c => !c.HasPassed))
				return false;

			certStatus.IsComplete = true;
			certStatus.CompletedAt = DateTime.UtcNow;
			var result = await _repository.UpdateAsync(CertificationStatusController.CollectionName,
				x => x.Id == certStatus.Id, certStatus);
			return await _messenger.SendMessage(CertificationCompletionName, certStatus.ToCertificationCompletion());
		}
	}
}
