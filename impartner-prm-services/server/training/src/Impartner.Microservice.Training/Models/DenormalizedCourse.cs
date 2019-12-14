namespace Impartner.Microservice.Training.Models
{
	public class DenormalizedCourse : TrainingBase
	{
		public DenormalizedCourse() { }

		public DenormalizedCourse(Course course)
		{
			Id = course.Id;
			IsActive = course.IsActive;
			Name = course.Name;
		}
	}
}
