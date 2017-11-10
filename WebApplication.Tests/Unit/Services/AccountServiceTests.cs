using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Config;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Services;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using Moq;
using NLog;
using System;
using System.IO;
using System.Web;
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

        private const string InvalidLogin = "joebloggs";
        private const string LoginLockedOut = "wolf";
        private const string ValidLogin = "simon";

        public AccountServiceTests()
        {
            HttpContext.Current = new HttpContext(
                new HttpRequest("", "http://tempuri.org", ""),
                new HttpResponse(new StringWriter())
            );

            _service = new AccountService(_userRepository.Object, _config.Object, _mailer.Object, _authentication.Object, _logger.Object);

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

    }

}
