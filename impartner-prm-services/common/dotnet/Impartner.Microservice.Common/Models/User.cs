using System.ComponentModel.DataAnnotations;

namespace Impartner.Microservice.Common.Models
{
	public class User
	{
		public string Username { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserId { get; set; }
	}
}
