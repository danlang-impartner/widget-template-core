using Impartner.Microservice.Common;
using Microsoft.AspNetCore.Hosting;

namespace Impartner.Microservice.DynamicForms
{
	public class Program
	{
		public static void Main(string[] args) =>
			ImpartnerWebHost
				.CreateWebHostBuilder<Startup>(args)            
				.Build()
				.Run();
	}
}
