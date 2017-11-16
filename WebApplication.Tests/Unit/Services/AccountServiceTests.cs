using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Services;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Tests.Shared;
using Moq;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Services
{
    public class AccountServiceTests
    {
        private readonly AccountService _service;
        readonly Mock<IRepository<User>> _userRepository = new Mock<IRepository<User>>();
        readonly Mock<IOptions<WebsiteConfiguration>> _config = new Mock<IOptions<WebsiteConfiguration>>();
        readonly Mock<IMailer> _mailer = new Mock<IMailer>();
        readonly Mock<IAuthentication> _authentication = new Mock<IAuthentication>();
        readonly Mock<ILogger> _logger = new Mock<ILogger>();
        readonly Mock<IRoles> _roles = new Mock<IRoles>();
        
        private const string InvalidLogin = "joebloggs";
        private const string LoginLockedOut = "wolf";
        private const string ValidLogin = "simon";

        public AccountServiceTests()
        {
            _config.SetupGet(_ => _.Value)
                .Returns(new WebsiteConfiguration
                {
                    CompanyLogoUrl = "http://test",
                    CompanyName = "Test Company"
                });

            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _service = new AccountService(_userRepository.Object, _config.Object, _mailer.Object, _authentication.Object, _logger.Object, _roles.Object);
            _service.UrlHelper = UrlTestHelper.CreaTestHelper();

            _authentication.Setup(_ => _.Login(InvalidLogin, It.IsAny<string>(), false))
                .Returns(false);
            _authentication.Setup(_ => _.Login(ValidLogin, It.IsAny<string>(), false))
                .Returns(true);
            _authentication.Setup(_ => _.IsAccountLockedOut(LoginLockedOut, 10, It.IsAny<TimeSpan>()))
                .Returns(true);
        }

        [Fact]
        public void Login_ShouldReturnAccountLocked()
        {
            Assert.Equal(ELoginResult.AccountLocked, _service.Login(LoginLockedOut, "sdlfkj", false));
        }

        [Fact]
        public void Login_ShouldReturnFail()
        {
            var password = "sdlfkj";    
            Assert.Equal(ELoginResult.Fail, _service.Login(InvalidLogin, password, false));
            _authentication.Verify(_ => _.IsAccountLockedOut(InvalidLogin, It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public void Login_Success()
        {
            var password = "sdlfkj";
            Assert.Equal(ELoginResult.Success, _service.Login(ValidLogin, password, false));
            _authentication.Verify(_ => _.Login(ValidLogin, password, false), Times.Once);
            _authentication.Verify(_ => _.IsAccountLockedOut(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public void Register_ShouldHaveErrors()
        {
            var username = "joebloggs";
            var email = "test@test.com";

            var model = new UserAccount.RegisterModel
            {
                UserName = username,
                EmailAddress = email,
                Password = "",
                ConfirmPassword = "123654"
            };

            _userRepository.Setup(_ => _.Exists(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(true);
            
            var result = _service.Register(model);

            Assert.Contains("UserName", result.Errors.Select(_ => _.FieldName));
            Assert.Contains("EmailAddress", result.Errors.Select(_ => _.FieldName));
            Assert.Contains("InvalidPassword", result.Errors.Select(_ => _.FieldName));
            Assert.Contains("ConfirmPassword", result.Errors.Select(_ => _.FieldName));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Register_ThrowsMembershipCreateUserException()
        {
            var username = "joebloggs";
            var email = "test@test.com";

            var model = new UserAccount.RegisterModel
            {
                UserName = username,
                EmailAddress = email,
                Password = "123456",
                ConfirmPassword = "123456"
            };

            _authentication.Setup(_ =>
                    _.CreateUserAndAccount(model.UserName, model.Password, It.IsAny<object>(), It.IsAny<bool>()))
                .Throws(new MembershipCreateUserException(MembershipCreateStatus.InvalidAnswer));

            var result = _service.Register(model);

            Assert.Contains(Dictionary.InvalidPasswordRetreivalError, result.Errors.Select(_ => _.ErrorMessage));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Register_ThrowsException()
        {
            var username = "joebloggs";
            var email = "test@test.com";

            var model = new UserAccount.RegisterModel
            {
                UserName = username,
                EmailAddress = email,
                Password = "123456",
                ConfirmPassword = "123456"
            };

            _authentication.Setup(_ =>
                    _.CreateUserAndAccount(model.UserName, model.Password, It.IsAny<object>(), It.IsAny<bool>()))
                .Throws(new Exception("Foo"));

            var result = _service.Register(model);

            Assert.Contains("Foo", result.Errors.Select(_ => _.ErrorMessage));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void Register_Success()
        {
            var username = "joebloggs";
            var email = "test@test.com";

            var model = new UserAccount.RegisterModel
            {
                UserName = username,
                EmailAddress = email,
                Password = "123456",
                ConfirmPassword = "123456",
                FirstName = "Joe",
                LastName = "Bloggs",
                PhoneNumber = "12323423",
                BirthDate = new DateTime(2017, 3, 3)
            };
            
            var result = _service.Register(model);

            Assert.Equal(0, result.Errors.Count);
            _authentication.Verify(_ => _.CreateUserAndAccount(model.UserName, model.Password, It.IsAny<object>(), true), Times.Once);
            _mailer.Verify(_ => _.SendEmail(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), true), 
                Times.Once);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void UpdatePassword_Fail()
        {
            _authentication.Setup(_ =>
                    _.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(false);

            var result = _service.UpdatePassword(new UserAccount.LocalPasswordModel
            {
              OldPassword  = "1234",
              NewPassword = "4321"
            });

            Assert.Contains(Dictionary.CurrentPasswordCorrectNewInvalidError, result.Errors.Select(_ => _.ErrorMessage));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void UpdatePassword_Error()
        {
            _authentication.Setup(_ =>
                    _.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("oops"));

            var result = _service.UpdatePassword(new UserAccount.LocalPasswordModel
            {
                OldPassword = "1234",
                NewPassword = "4321"
            });

            Assert.Contains(Dictionary.UpdatePaswordError, result.Errors.Select(_ => _.ErrorMessage));
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void UpdatePassword_Success()
        {
            _authentication.Setup(_ =>
                    _.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            var currentUsername = "joebloogs";
            _authentication.SetupGet(_ => _.CurrentUserName)
                .Returns(currentUsername);

            var localPasswordModel = new UserAccount.LocalPasswordModel
            {
                OldPassword = "1234",
                NewPassword = "4321"
            };  
            var result = _service.UpdatePassword(localPasswordModel);

            Assert.Equal(0, result.Errors.Count);
            _authentication.Verify(_ => _.ChangePassword(currentUsername, localPasswordModel.OldPassword, localPasswordModel.NewPassword), Times.Once);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void PasswordResetRequest_Fail()
        {
            var passwordResetRequestModel = new UserAccount.PasswordResetRequestModel
            {
                EmailAddress = "jloggs@test.com",
                UserName = "jbloggs"
            };

            var result = _service.PasswordResetRequest(passwordResetRequestModel);

            Assert.False(result.IsSuccess);
            Assert.Contains(Dictionary.InvalidUsernameError, result.Errors.Select(_ => _.ErrorMessage));
        }

        [Fact]
        public void PasswordResetRequest_Success()
        {
            var model = new UserAccount.PasswordResetRequestModel
            {
                EmailAddress = "jloggs@test.com",
                UserName = "jbloggs"
            };
            var item = new User
            {
                Username = model.UserName,
                EmailAddress = model.EmailAddress,
                FirstName = "Joe",
                LastName = "Bloggs",
                FullName = "Joe Bloggs"
            };
            _userRepository.Setup(_ => _.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>
                {
                    item
                }.AsQueryable());
            var token = "token";
            _authentication.Setup(_ => _.GeneratePasswordResetToken(model.UserName, It.IsAny<int>()))
                .Returns(token);

            var result = _service.PasswordResetRequest(model);

            Assert.True(result.IsSuccess);
            Assert.Equal(token, result.Data);
            _authentication.Verify(_ => _.GeneratePasswordResetToken(model.UserName, It.IsAny<int>()), Times.Once);
            _mailer.Verify(_ => _.SendEmail(Dictionary.PasswordResetTitle, It.IsAny<string>(), model.EmailAddress, "Joe Bloggs", It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once);
        }

        [InlineData(1, 2, false)]
        [InlineData(2, 2, true)]
        [Theory]
        public void ConfirmUserFromToken_Fail(int userId1, int userId2, bool result)
        {
            _authentication.Setup(_ => _.GetUserIdFromPasswordResetToken(It.IsAny<string>()))
                .Returns(userId1);
            _authentication.Setup(_ => _.GetUserId(It.IsAny<string>()))
                .Returns(userId2);

            Assert.Equal(result, _service.ConfirmUserFromToken("blaa", "token"));
        }

        [Fact]
        public void ResetPassword_Error()
        {
            _authentication.Setup(_ => _.ResetPassword(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("oops"));

            var result = _service.ResetPassword(new UserAccount.ResetPasswordModel());

            Assert.Equal(1, result.Errors.Count);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public void ResetPassword_Success()
        {
            Assert.True(_service.ResetPassword(new UserAccount.ResetPasswordModel()).IsSuccess);
        }

        [Fact]
        public void ActivateAccount_Fail()
        {
            var result = _service.ActivateAccount(3, "token");
            Assert.Equal(EActivateAccountResult.Fail, result.Result);
        }
        
        [Fact]
        public void ActivateAccount_Success()
        {
            var item = new User
            {
                Username = "jbloggs",
                EmailAddress = "jbloggs@test.com",
                FirstName = "Joe",
                Name = "Joe Bloggs"
            };
            item.Validated();
            _userRepository.Setup(_ => _.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>
                {
                    item
                }.AsQueryable());
            _authentication.Setup(_ => _.ConfirmAccount(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var token = "token";    
            var result = _service.ActivateAccount(3, token);

            Assert.Equal(EActivateAccountResult.Success, result.Result);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public void ActivateAccountByUsername_Fail()
        {
            var result = _service.ActivateAccount("username", "token");
            Assert.Equal(EActivateAccountResult.Fail, result.Result);
        }

        [Fact]
        public void ActivateAccountbyUsername_Success()
        {
            _userRepository.Setup(_ => _.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>
                {
                    new User
                    {
                        Username = "jbloggs",
                        EmailAddress = "jbloggs@test.com",
                        FirstName = "Joe",
                        Name = "Joe Bloggs"
                    }
                }.AsQueryable());
            _authentication.Setup(_ => _.ConfirmAccount(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);

            var token = "token";
            var result = _service.ActivateAccount("username", token);

            Assert.Equal(EActivateAccountResult.Success, result.Result);
            Assert.Equal(token, result.Token);
        }

        [Fact]
        public void ActivateAccount_AlreadyActivated()
        {
            var token = "token";
            var user = new User
            {
                Id = 3,
                Username = "jbloggs"
            };
            _userRepository.Setup(_ => _.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>
                {
                    new User
                    {
                        Username = "jbloggs",
                        EmailAddress = "jbloggs@test.com",
                        FirstName = "Joe",
                        Name = "Joe Bloggs"
                    }
                }.AsQueryable());
            _authentication.Setup(_ => _.ConfirmAccount(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(true);
            _authentication.Setup(_ => _.IsConfirmed(user.Username))
                .Returns(true);
                
            var result = _service.ActivateAccount(user, token);

            Assert.Equal(EActivateAccountResult.AlreadyActivated, result.Result);
        }

        [Fact]
        public void ActivateAccountByUser_Fail()
        {
            var token = "token";
            var user = new User
            {
                Id = 3,
                Username = "jbloggs"
            };
            _userRepository.Setup(_ => _.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>
                {
                    new User
                    {
                        Username = "jbloggs",
                        EmailAddress = "jbloggs@test.com",
                        FirstName = "Joe",
                        Name = "Joe Bloggs"
                    }
                }.AsQueryable());
            _authentication.Setup(_ => _.ConfirmAccount(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(false);
            _authentication.Setup(_ => _.IsConfirmed(user.Username))
                .Returns(false);

            var result = _service.ActivateAccount(user, token);

            Assert.Equal(EActivateAccountResult.Fail, result.Result);
        }

        [Fact]
        public void Logout_LogsOut()
        {
            _service.Logout();
            _authentication.Verify(_ => _.Logout(), Times.Once);
        }

        [Fact]
        public void GetAccountActivationToken_HappyPath()
        {
            var token = "bak";
            _userRepository.Setup(_ => _.CustomQuery<string>(It.IsAny<string>()))
                .Returns(new List<string>
                {
                    token
                });
            Assert.Equal(token, _service.GetAccountActivationToken(3));
        }
    }

}
