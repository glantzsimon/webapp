using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Enums;

namespace K9.Base.WebApplication.Models
{
	public class ActivateAccountResult
	{
		public EActivateAccountResult Result { get; set; }
		public User User { get; set; }
		public string Token { get; set; }
	}
}