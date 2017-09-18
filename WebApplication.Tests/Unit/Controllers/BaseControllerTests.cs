using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Exceptions;
using K9.Base.WebApplication.Helpers;
using K9.Base.WebApplication.Models;
using K9.Base.WebApplication.UnitsOfWork;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Tests.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using K9.SharedLibrary.Helpers;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class BaseControllerTests
    {
        public static readonly Mock<HttpResponseBase> MockHttpResponse = new Mock<HttpResponseBase>();
        public static readonly Mock<ControllerContext> MockControllerContext = new Mock<ControllerContext>();
        public const int FilteredUserId = 2;

        public class MockController<T> : BaseController<T>
            where T : class, IObjectBase
        {
            public StreamWriter TextWriter { get; }

            public MockController(IControllerPackage<T> controllerPackage) : base(controllerPackage)
            {
                TextWriter = new StreamWriter(new MemoryStream());

                var routeData = new RouteData();
                routeData.Values.Add("controller", "PersonsController");
                var view = new Mock<IView>();
                var engine = new Mock<IViewEngine>();
                var viewEngineResult = new ViewEngineResult(view.Object, engine.Object);

                MockControllerContext.SetupGet(_ => _.HttpContext.User.Identity.Name)
                    .Returns("admin");
                MockControllerContext.SetupGet(_ => _.HttpContext.Request.IsAuthenticated)
                    .Returns(true);
                MockControllerContext.SetupGet(_ => _.HttpContext.Request.QueryString)
                    .Returns(new NameValueCollection
                {
                    { "fkName", "UserId" },
                    { "fkValue", $"{FilteredUserId}" }
                });
                MockControllerContext.SetupGet(_ => _.HttpContext.User.Identity.IsAuthenticated)
                    .Returns(true);
                MockControllerContext.SetupGet(m => m.RouteData)
                    .Returns(routeData);
                MockControllerContext.SetupGet(_ => _.HttpContext.Response)
                    .Returns(MockHttpResponse.Object);
                engine.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(viewEngineResult);

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(engine.Object);
                ViewData = new ViewDataDictionary();
                ControllerContext = MockControllerContext.Object;
            }
        }

        public class PersonsController : MockController<Person>
        {
            public PersonsController(IControllerPackage<Person> controllerPackage) : base(controllerPackage)
            {
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
        private readonly PersonsController _personController;
        private readonly MockLimitedByUserErrorController _limitedErrorController;
        private readonly MockLimitedByUserController _limitedController;
        private readonly Mock<IControllerPackage<Person>> _package = new Mock<IControllerPackage<Person>>();
        private readonly Mock<IControllerPackage<PersonWithIUserData>> _limitedPackage = new Mock<IControllerPackage<PersonWithIUserData>>();
        private readonly Mock<IDataSetsHelper> _datasetsHelper = new Mock<IDataSetsHelper>();
        private readonly Mock<IDataTableAjaxHelper<Person>> _ajaxHelper = new Mock<IDataTableAjaxHelper<Person>>();
        private readonly Mock<IFileSourceHelper> _fileSourceHelper = new Mock<IFileSourceHelper>();
        private const int InvalidId = 2;
        private const int ValidId = 3;
        private const int ValidUserId = 7;
        private const int CurrentUserId = 178;

        private void SetupPackage<T>(Mock<IControllerPackage<T>> package)
            where T : class, IObjectBase
        {
            package.SetupGet(_ => _.Authentication)
              .Returns(_authentication.Object);
            package.SetupGet(_ => _.Roles)
                .Returns(_roles.Object);
            package.SetupGet(_ => _.FileSourceHelper)
                .Returns(_fileSourceHelper.Object);
        }

        private void SetupRepository<T>(Mock<IRepository<T>> repository)
            where T : class, IObjectBase
        {
            var validEntity = Activator.CreateInstance<T>();
            validEntity.SetProperty("Id", ValidId);
            validEntity.SetProperty("Name", "John");
            validEntity.SetProperty("UserId", ValidUserId);

            repository.Setup(_ => _.GetName(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("Gizzie");
            repository.Setup(_ => _.Find(InvalidId))
                .Returns((T)null);
            repository.Setup(_ => _.Find(ValidId))
                .Returns(validEntity);
            repository.Setup(_ => _.Find(ValidId))
                .Returns(validEntity);
        }

        public BaseControllerTests()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("gb");
            _personController = new PersonsController(_package.Object);
            _limitedErrorController = new MockLimitedByUserErrorController(_package.Object);
            _limitedController = new MockLimitedByUserController(_limitedPackage.Object);

            SetupPackage(_package);
            SetupPackage(_limitedPackage);
            _package.SetupGet(_ => _.Repository)
                .Returns(_repository.Object);
            _package.SetupGet(_ => _.DataSetsHelper)
                .Returns(_datasetsHelper.Object);

            _package.SetupGet(_ => _.AjaxHelper)
                .Returns(_ajaxHelper.Object);
            _limitedPackage.SetupGet(_ => _.Repository)
                .Returns(_limitedRepository.Object);

            SetupRepository(_repository);
            SetupRepository(_limitedRepository);
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
                .Returns(CurrentUserId);
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
            Assert.Equal("Person Details", _personController.ViewBag.SubTitle);
            Assert.Equal("", view.ViewName);

            var crumb = (_personController.ViewBag.Crumbs as List<Crumb>).First();
            Assert.Equal("Persons", crumb.Label);
            Assert.Equal("Index", crumb.ActionName);
            Assert.Equal("Persons", crumb.ControllerName);
            Assert.Equal(ValidId, ((Person)view.Model).Id);
        }

        [Fact]
        public void List_HappyPath()
        {
            var querystring = new NameValueCollection
            {
                {"draw", "1"},
                {"start", "0"},
                {"length", "10"},
                {"search[value]", "search"},
                {"search[regex]", "true"},
                {"order[0][column]", "1"},
                {"order[0][dir]", "asc"},
                {"columns[0][data]", "UserId"},
                {"columns[0][name]", "User"},
                {"columns[0][search][value]", ""},
                {"columns[0][search][regex]", "false"},
                {"columns[1][data]", "UserName"},
                {"columns[1][name]", "User Name"},
                {"columns[1][search][value]", ""},
                {"columns[1][search][regex]", "false"}
            };

            var count = 32;
            var filteredCount = 24;
            var personsList = new List<Person>();

            for (int i = 0; i < count; i++)
            {
                personsList.Add(new Person
                {
                    Id = i
                });
            }

            var whereClause = " WHERE UserId = 178";

            MockControllerContext.SetupGet(_ => _.HttpContext.Request.QueryString)
                .Returns(querystring);
            _repository.Setup(_ => _.GetCount(string.Empty))
                .Returns(count);
            _repository.Setup(_ => _.GetQuery(It.IsAny<string>()))
                .Returns(personsList);
            _ajaxHelper.Setup(_ => _.GetWhereClause(true, null))
                .Returns(whereClause);
            _repository.Setup(_ => _.GetCount(whereClause))
                .Returns(filteredCount);

            var contentResult = Assert.IsType<ContentResult>(_personController.List());

            Assert.Contains("\"draw\":0", contentResult.Content);
            Assert.Contains($"\"recordsTotal\":{count}", contentResult.Content);
            Assert.Contains($"\"recordsFiltered\":{filteredCount}", contentResult.Content);
            Assert.Contains($"\"Id\":{personsList.Last().Id}", contentResult.Content);
        }

        [Fact]
        public void Create_ShouldUpateUserId_IfStatelessFilterIsSet()
        {
            var viewResult = Assert.IsType<ViewResult>(_personController.Create());
            var model = viewResult.Model as Person;

            Assert.Equal("Persons", _personController.ViewBag.Title);
            Assert.Equal("Create New Person for Gizzie", _personController.ViewBag.SubTitle);

            Assert.Equal(FilteredUserId, model.UserId);
        }

        [Fact]
        public void Create_ShouldUpateUserId_ToAuthenticatedUser_IfImplementsIUserData()
        {
            MockControllerContext.SetupGet(_ => _.HttpContext.Request.QueryString)
                .Returns(new NameValueCollection());
            _authentication.SetupGet(_ => _.CurrentUserId)
                .Returns(CurrentUserId);

            PersonWithIUserData modelSentToEvent = null;

            _limitedController.RecordBeforeCreate += (sender, e) =>
            {
                modelSentToEvent = (PersonWithIUserData)e.Item;
            };

            var viewResult = Assert.IsType<ViewResult>(_limitedController.Create());
            var model = viewResult.Model as PersonWithIUserData;

            Assert.Equal("PersonWithIUserDatas", _limitedController.ViewBag.Title);
            Assert.Equal("Create New PersonWithIUserData for Gizzie", _limitedController.ViewBag.SubTitle);
            Assert.Equal(CurrentUserId, model.UserId);

            var crumb = (_limitedController.ViewBag.Crumbs as List<Crumb>).First();
            Assert.Equal("PersonWithIUserDatas", crumb.Label);
            Assert.Equal("Index", crumb.ActionName);
            Assert.Equal("MockLimitedByUser", crumb.ControllerName);
            Assert.Equal(0, (model).Id);
            Assert.Equal(modelSentToEvent, model);
            Assert.Equal("", viewResult.ViewName);
        }

        [Fact]
        public void CreatePost_ModelInvalid()
        {
            var person = new Person
            {
                Id = 3,
                Name = "John"
            };
            _personController.ModelState.AddModelError("", "Something is wrong");

            var viewResult = Assert.IsType<ViewResult>(_personController.Create(person));

            Assert.Equal("", viewResult.ViewName);
            Assert.Equal(person, viewResult.Model);
            Assert.Equal("Persons", _personController.ViewBag.Title);
            Assert.Equal("Persons", _personController.ViewBag.Title);
            Assert.Equal("Create New Person for Gizzie", _personController.ViewBag.SubTitle);
        }

        [Fact]
        public void CreatePost_Success()
        {
            var fileSource = new FileSource
            {
                PostedFile = new List<HttpPostedFileBase>
                {
                    new Mock<HttpPostedFileBase>().Object
                }
            };
            var person = new Person
            {
                Id = 3,
                Name = "John",
                Photos = fileSource
            };
            Person modelSentToEvent = null;
            _personController.RecordBeforeCreated += (sender, e) =>
            {
                modelSentToEvent = (Person)e.Item;
            };

            var redirectResult = Assert.IsType<RedirectToRouteResult>(_personController.Create(person));

            Assert.Equal(modelSentToEvent, person);
            Assert.Equal("Index", redirectResult.RouteValues["action"]);
            _repository.Verify(_ => _.Create(person), Times.Once);
            _fileSourceHelper.Verify(_ => _.SaveFilesToDisk(fileSource, It.IsAny<bool>()), Times.Once);

        }

    }

}
