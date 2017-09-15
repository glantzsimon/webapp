using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Models;

namespace K9.Base.WebApplication.Services
{
    public interface IAccountService
    {
		ELoginResult Login(string username, string password, bool isRemember);
		void Logout();
		ServiceResult Register(UserAccount.RegisterModel model);
		ServiceResult UpdatePassword(UserAccount.LocalPasswordModel model);
		ServiceResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model);
		bool ConfirmUserFromToken(string username, string token);
		ServiceResult ResetPassword(UserAccount.ResetPasswordModel model);
		ActivateAccountResult ActivateAccount(int userId, string token = "");
		ActivateAccountResult ActivateAccount(string username, string token = "");
		ActivateAccountResult ActivateAccount(User user, string token = "");
		string GetAccountActivationToken(int userId);
	}
}