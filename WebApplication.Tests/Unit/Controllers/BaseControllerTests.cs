using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Models;
using K9.WebApplication.Tests.Models;
using Moq;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using K9.Base.WebApplication.Exceptions;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class BaseControllerTests
    {
        public static readonly Mock<HttpResponseBase> MockHttpResponse = new Mock<HttpResponseBase>();

        public class MockController<T> : BaseController<T> 
            where T : class , IObjectBase
        {
            public StreamWriter TextWriter { get; }

            public MockController(IControllerPackage<T> controllerPackage) : base(controllerPackage)
            {
                TextWriter = new StreamWriter(new MemoryStream());
                var controllerContext = new Mock<ControllerContext>();
                var routeData = new RouteData();
                routeData.Values.Add("controller", "PersonsController");
                var view = new Mock<IView>();
                var engine = new Mock<IViewEngine>();
                var viewEngineResult = new ViewEngineResult(view.Object, engine.Object);

                controllerContext.SetupGet(_ => _.HttpContext.User.Identity.Name)
                    .Returns("admin");
                controllerContext.SetupGet(_ => _.HttpContext.Request.IsAuthenticated)
                    .Returns(true);
                controllerContext.SetupGet(_ => _.HttpContext.Request.QueryString)
                    .Returns(new NameValueCollection
                {
                    { "fkName", "UserId" },
                    { "fkValue", "2" }
                });
                controllerContext.SetupGet(_ => _.HttpContext.User.Identity.IsAuthenticated)
                    .Returns(true);
                controllerContext.SetupGet(m => m.RouteData)
                    .Returns(routeData);
                controllerContext.SetupGet(_ => _.HttpContext.Response)
                    .Returns(MockHttpResponse.Object);
                engine.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(viewEngineResult);

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(engine.Object);
                ViewData = new ViewDataDictionary();
                ControllerContext = controllerContext.Object;
            }
        }

        [LimitByUserId]
        public class MockLimitedByUserErrorController : MockController<Person>
        {
            public MockLimitedByUserErrorController(IControllerPackage<Person> controllerPackage) : base(controllerPackage)
            {
            }
        }

        [LimitByUserId]
        public class MockLimitedByUserController : MockController<PersonWithIUserData>
        {
            public MockLimitedByUserController(IControllerPackage<PersonWithIUserData> controllerPackage) : base(controllerPackage)
            {
            }
        }

        private readonly Mock<IRepository<Person>> _repository = new Mock<IRepository<Person>>();
        private readonly Mock<IRepository<PersonWithIUserData>> _limitedRepository = new Mock<IRepository<PersonWithIUserData>>();
        private readonly Mock<IAuthentication> _authentication = new Mock<IAuthentication>();
        private readonly Mock<IRoles> _roles = new Mock<IRoles>();
        private readonly MockController<Person> _personController;
        private readonly MockLimitedByUserErrorController _limitedErrorController;
        private readonly MockLimitedByUserController _limitedController;
        private readonly Mock<IControllerPackage<Person>> _package = new Mock<IControllerPackage<Person>>();
        private readonly Mock<IControllerPackage<PersonWithIUserData>> _limitedPackage = new Mock<IControllerPackage<PersonWithIUserData>>();
        private const int InvalidId = 2;
        private const int ValidId = 3;
        private const int ValidUserId = 7;

        private void SetupPackage<T>(Mock<IControllerPackage<T>> package)
            where T : class , IObjectBase
        {
              package.SetupGet(_ => _.Authentication)
                .Returns(_authentication.Object);
            package.SetupGet(_ => _.Roles)
                .Returns(_roles.Object);
        }

        public BaseControllerTests()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("gb");
            _personController = new MockController<Person>(_package.Object);
            _limitedErrorController = new MockLimitedByUserErrorController(_package.Object);
            _limitedController = new MockLimitedByUserController(_limitedPackage.Object);

            SetupPackage(_package);
            SetupPackage(_limitedPackage);

            _package.SetupGet(_ => _.Repository)
                .Returns(_repository.Object);
            _limitedPackage.SetupGet(_ => _.Repository)
                .Returns(_limitedRepository.Object);

            _repository.Setup(_ => _.GetName(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("Gizzie");
            _repository.Setup(_ => _.Find(InvalidId))
                .Returns((Person)null);
            _repository.Setup(_ => _.Find(ValidId))
                .Returns(new Person
                {
                    Id = ValidId,
                    Name = "John",
                    UserId = ValidUserId
                });
            _limitedRepository.Setup(_ => _.Find(ValidId))
                .Returns(new PersonWithIUserData
                {
                    Id = ValidId,
                    Name = "John",
                    UserId = ValidUserId
                });
        }

        [Fact]
        public void Index_ShouldSetTitle_AndSubTitle()
        {
            _personController.Index();

            Assert.Equal("Persons", _personController.ViewBag.Title);
            Assert.Equal("Persons for Gizzie", _personController.ViewBag.SubTitle);
        }

        [Fact]
        public void Details_ShouldReturnHttpNotFoundIfItemNotFound()
        {
            var statuscode = 0;

            MockHttpResponse.SetupSet(_ => _.StatusCode = It.IsAny<int>())
                .Callback<int>(value => statuscode = value);

            var view = Assert.IsType<ViewResult>(_personController.Details(InvalidId));

            Assert.Equal("NotFound", view.ViewName);
            Assert.Equal((int)HttpStatusCode.NotFound, statuscode);
        }

        [Fact]
        public void WhenLimitedByUser_AndNonAdminUserLoggedIn_AndObjectBaseDoesNotSupportLimiting_ThrowError()
        {
            _authentication.SetupGet(_ => _.IsAuthenticated)
                .Returns(true);
            _roles.Setup(_ => _.CurrentUserIsInRoles(RoleNames.Administrators))
                .Returns(false);
          
            Assert.Throws<LimitByUserIdException>(() => _limitedErrorController.Details(ValidId));
        }

        [Fact]
        public void WhenLimitedByUser_AndNonAdminUserLoggedIn_AndObjectBaseSupportsLimiting_ButUserIdIsDifferentFromRecordUserId()
        {
            var statuscode = 0;

            MockHttpResponse.SetupSet(_ => _.StatusCode = It.IsAny<int>())
                .Callback<int>(value => statuscode = value);
            _authentication.SetupGet(_ => _.IsAuthenticated)
                .Returns(true);
            _authentication.SetupGet(_ => _.CurrentUserId)
                .Returns(178);
            _roles.Setup(_ => _.CurrentUserIsInRoles(RoleNames.Administrators))
                .Returns(false);

            var view = Assert.IsType<ViewResult>(_limitedController.Details(ValidId));

            Assert.Equal("Unauthorized", view.ViewName);
            Assert.Equal((int)HttpStatusCode.Forbidden, statuscode);
        }

        [Fact]
        public void Details_HappyPath()
        {
            var view = Assert.IsType<ViewResult>(_personController.Details(ValidId));

            Assert.Equal("Persons", _personController.ViewBag.Title);
            Assert.Null(view.ViewName);
        }


    }

}
