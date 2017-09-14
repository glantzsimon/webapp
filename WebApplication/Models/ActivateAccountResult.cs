using K9.Base.WebApplication.Enums;
using K9.DataAccessLayer.Models;

namespace K9.Base.WebApplication.Models
{
	public class ActivateAccountResult
	{
		public EActivateAccountResult Result { get; set; }
		public User User { get; set; }
		public string Token { get; set; }
	}
}