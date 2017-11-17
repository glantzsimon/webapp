using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Models;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using NLog;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace K9.Base.WebApplication.Services
{
    public class AccountService : IAccountService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IMailer _mailer;
        private readonly IAuthentication _authentication;
        private readonly ILogger _logger;
        private readonly IRoles _roles;
        private readonly WebsiteConfiguration _config;
        private UrlHelper _urlHelper;

        public UrlHelper UrlHelper
        {
            get => _urlHelper;
            set => _urlHelper = value;
        }

        public AccountService(IRepository<User> userRepository, IOptions<WebsiteConfiguration> config, IMailer mailer, IAuthentication authentication, ILogger logger, IRoles roles)
        {
            _userRepository = userRepository;
            _mailer = mailer;
            _authentication = authentication;
            _logger = logger;
            _roles = roles;
            _config = config.Value;
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public ELoginResult Login(string username, string password, bool isRemember)
        {
            if (_authentication.Login(username, password, isRemember))
            {
                return ELoginResult.Success;
            }
            if (_authentication.IsAccountLockedOut(username, 10, TimeSpan.FromDays(1)))
            {
                return ELoginResult.AccountLocked;
            }
            return ELoginResult.Fail;
        }

        public ServiceResult Register(UserAccount.RegisterModel model)
        {
            var result = new ServiceResult();

            if (_userRepository.Exists(u => u.Username == model.UserName))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "UserName",
                    ErrorMessage = Dictionary.UsernameIsUnavailableError
                });
            }
            if (_userRepository.Exists(u => u.EmailAddress == model.EmailAddress))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "EmailAddress",
                    ErrorMessage = Dictionary.EmailIsUnavailableError
                });
            }
            if (string.IsNullOrEmpty(model.Password))
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "InvalidPassword",
                    ErrorMessage = Dictionary.InvalidPasswordEnteredError
                });
            }
            if (model.Password != model.ConfirmPassword)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "ConfirmPassword",
                    ErrorMessage = Dictionary.PasswordMatchError
                });
            }

            if (!result.Errors.Any())
            {
                var newUser = new
                {
                    model.EmailAddress,
                    model.FirstName,
                    model.LastName,
                    model.PhoneNumber,
                    model.BirthDate,
                    Name = Guid.NewGuid(),
                    FullName = $"{model.FirstName} {model.LastName}",
                    IsUnsubscribed = false,
                    IsSystemStandard = false,
                    CreatedBy = SystemUser.System,
                    CreatedOn = DateTime.Now,
                    LastUpdatedBy = SystemUser.System,
                    LastUpdatedOn = DateTime.Now
                };

                try
                {
                    var token = _authentication.CreateUserAndAccount(model.UserName, model.Password,
                        newUser, true);
                    SendActivationemail(model, token);
                    _roles.AddUserToRole(model.UserName, RoleNames.DefaultUsers);
                    result.IsSuccess = true;
                    return result;
                }
                catch (MembershipCreateUserException e)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ErrorCodeToString(e.StatusCode)
                    });
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ex.Message,
                        Exception = ex,
                        Data = newUser
                    });
                }
            }

            return result;
        }

        public ServiceResult DeleteAccount(int userId)
        {
            var result = new ServiceResult();
            var user = _userRepository.Find(userId);

            if (user == null || _authentication.CurrentUserName != user.Username)
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = "Username",
                    ErrorMessage = Dictionary.UserNotFoundError
                });
            }

            if (!result.Errors.Any())
            {
                try
                {
                    user.SetToDeleted();
                    _userRepository.Update(user);
                    result.IsSuccess = true;
                    return result;
                }
                catch (Exception ex)
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = ex.Message,
                        Exception = ex,
                        Data = user
                    });
                }
            }

            return result;
        }

        public ServiceResult UpdatePassword(UserAccount.LocalPasswordModel model)
        {
            var result = new ServiceResult();
            try
            {
                if (_authentication.ChangePassword(_authentication.CurrentUserName, model.OldPassword, model.NewPassword))
                {
                    result.IsSuccess = true;
                }
                else
                {
                    result.Errors.Add(new ServiceError
                    {
                        FieldName = "",
                        ErrorMessage = Dictionary.CurrentPasswordCorrectNewInvalidError
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = Dictionary.UpdatePaswordError
                });
            }
            return result;
        }

        public ServiceResult PasswordResetRequest(UserAccount.PasswordResetRequestModel model)
        {
            var result = new ServiceResult();
            var user = _userRepository.Find(u => u.EmailAddress == model.EmailAddress).FirstOrDefault();

            if (user != null)
            {
                try
                {
                    model.UserName = user.Username;
                    var token = _authentication.GeneratePasswordResetToken(user.Username);
                    SendPasswordResetEmail(model, token);
                    result.IsSuccess = true;
                    result.Data = token;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.GetFullErrorMessage());
                }
            }
            else
            {
                result.Errors.Add(new ServiceError
                {
                    FieldName = nameof(UserAccount.PasswordResetRequestModel.UserName),
                    ErrorMessage = Dictionary.InvalidUsernameError
                });
            }

            return result;
        }

        public bool ConfirmUserFromToken(string username, string token)
        {
            var userId = _authentication.GetUserIdFromPasswordResetToken(token);
            var confirmUserId = _authentication.GetUserId(username);
            return userId == confirmUserId;
        }

        public ServiceResult ResetPassword(UserAccount.ResetPasswordModel model)
        {
            var result = new ServiceResult();

            try
            {
                _authentication.ResetPassword(model.Token, model.NewPassword);
                _authentication.Login(model.UserName, model.NewPassword);
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetFullErrorMessage());
                result.Errors.Add(new ServiceError
                {
                    FieldName = "",
                    ErrorMessage = Dictionary.PasswordResetFailError
                });
            }
            return result;
        }

        public ActivateAccountResult ActivateAccount(int userId, string token = "")
        {
            var user = _userRepository.Find(u => u.Id == userId).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user, token);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(string username, string token = "")
        {
            var user = _userRepository.Find(u => u.Username == username).FirstOrDefault();
            if (user != null)
            {
                return ActivateAccount(user, token);
            }
            return new ActivateAccountResult
            {
                Result = EActivateAccountResult.Fail
            };
        }

        public ActivateAccountResult ActivateAccount(User user, string token = "")
        {
            var result = new ActivateAccountResult
            {
                User = user
            };

            if (user != null)
            {
                if (_authentication.IsConfirmed(user.Username))
                {
                    _logger.Error("Account already activated for user '{0}'.", user.Username);
                    result.Result = EActivateAccountResult.AlreadyActivated;
                    return result;
                }

                if (string.IsNullOrEmpty(token))
                {
                    token = GetAccountActivationToken(user.Id);
                }
                if (!_authentication.ConfirmAccount(user.Username, token))
                {
                    _logger.Error("ActivateAccount failed as user '{0}' was not found.", user.Username);
                    result.Result = EActivateAccountResult.Fail;
                    return result;
                }

                result.Token = token;
                result.Result = EActivateAccountResult.Success;
            }

            return result;
        }

        public void Logout()
        {
            _authentication.Logout();
        }

        public string GetAccountActivationToken(int userId)
        {
            string sql = "SELECT ConfirmationToken FROM webpages_Membership " + $"WHERE UserId = {userId}";
            return _userRepository.CustomQuery<string>(sql).FirstOrDefault();
        }

        private string GetActivationLink(UserAccount.RegisterModel model, string token)
        {
            return _urlHelper.AsboluteAction("ActivateAccount", "Account", new { userName = model.UserName, token });
        }

        private void SendActivationemail(UserAccount.RegisterModel model, string token)
        {
            var activationLink = GetActivationLink(model, token);
            var imageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl);

            var emailContent = TemplateProcessor.PopulateTemplate(Dictionary.WelcomeEmail, new
            {
                Title = Dictionary.Welcome,
                FirstName = model.FirstName,
                Company = _config.CompanyName,
                ActivationLink = activationLink,
                ImageUrl = imageUrl,
                From = Dictionary.ClientServices
            });

            _mailer.SendEmail(Dictionary.AccountActivationTitle, emailContent, model.EmailAddress, model.GetFullName());
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return Dictionary.UsernameExistsError;

                case MembershipCreateStatus.DuplicateEmail:
                    return Dictionary.UserNameEmailExistsError;

                case MembershipCreateStatus.InvalidPassword:
                    return Dictionary.InvalidPasswordEnteredError;

                case MembershipCreateStatus.InvalidEmail:
                    return Dictionary.InvalidEmailError;

                case MembershipCreateStatus.InvalidAnswer:
                    return Dictionary.InvalidPasswordRetreivalError;

                case MembershipCreateStatus.InvalidQuestion:
                    return Dictionary.InvalidRetrievalQuestionError;

                case MembershipCreateStatus.InvalidUserName:
                    return Dictionary.InvalidUsernameError;

                case MembershipCreateStatus.ProviderError:
                    return Dictionary.ProviderError;

                case MembershipCreateStatus.UserRejected:
                    return Dictionary.UserRejectedError;

                default:
                    return Dictionary.DefaultAuthError;
            }
        }

        private string GetPasswordResetLink(UserAccount.PasswordResetRequestModel model, string token)
        {
            return _urlHelper.AsboluteAction("ResetPassword", "Account", new { userName = model.UserName, token });
        }

        private void SendPasswordResetEmail(UserAccount.PasswordResetRequestModel model, string token)
        {
            var resetPasswordLink = GetPasswordResetLink(model, token);
            var imageUrl = _urlHelper.AbsoluteContent(_config.CompanyLogoUrl);
            var user = _userRepository.Find(u => u.Username == model.UserName).FirstOrDefault();

            if (user == null)
            {
                _logger.Error("SendPasswordResetEmail failed as no user was found. PasswordResetRequestModel: {0}", model);
                throw new NullReferenceException("User cannot be null");
            }

            var emailContent = TemplateProcessor.PopulateTemplate(Dictionary.PasswordResetEmail, new
            {
                Title = Dictionary.Welcome,
                FirstName = user.FirstName,
                Company = _config.CompanyName,
                ResetPasswordLink = resetPasswordLink,
                ImageUrl = imageUrl,
                From = Dictionary.ClientServices
            });

            _mailer.SendEmail(Dictionary.PasswordResetTitle, emailContent, model.EmailAddress, user.FullName);
        }

    }
}