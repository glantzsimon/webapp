using K9.DataAccessLayer.Models;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Models
{
	public class ActivateAccountResult
	{
		public EActivateAccountResult Result { get; set; }
		public User User { get; set; }
		public string Token { get; set; }
	}
}