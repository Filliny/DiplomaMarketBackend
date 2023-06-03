using DiplomaMarketBackend.Entity.Models;
using Mapster;

namespace DiplomaMarketBackend.Entity.Models
{
	public class ReceiverModel
	{
		public int Id { get; set; }
		public string ProfileName { get; set; } = string.Empty;

		[AdaptMember("FirstName")]
		public string Name { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public string MiddleName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string Address { get; set; } = string.Empty;
		public string Phone { get; set; } = string.Empty;
		
		public string UserId { get; set; } = string.Empty;
		public UserModel? User { get; set; } 
	}
}
