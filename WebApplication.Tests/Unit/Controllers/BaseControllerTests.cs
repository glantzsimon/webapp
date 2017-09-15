using System;
using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.UnitsOfWork;
using Moq;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using K9.Base.DataAccessLayer.Models;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Models;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class BaseControllerTests
    {

        public class Person : ObjectBase
        {
            [ForeignKey("User")]
            public int UserId { get; set; }

            public virtual User User { get; set; }

            [LinkedColumn(LinkedTableName = "User", LinkedColumnName = "Username")]
            public string UserName { get; set; }
        }
        public class MockController : BaseController<Person>
        {
            public MockController(IControllerPackage<Person> controllerPackage) : base(controllerPackage)
            {
                var mock = new Mock<ControllerContext>();
                var routeData = new RouteData();
                routeData.Values.Add("controller", "PersonsController");
                var view = new Mock<IView>();
                var engine = new Mock<IViewEngine>();
                var viewEngineResult = new ViewEngineResult(view.Object, engine.Object);

                mock.SetupGet(_ => _.HttpContext.User.Identity.Name)
                    .Returns("admin");
                mock.SetupGet(_ => _.HttpContext.Request.IsAuthenticated)
                    .Returns(true);
                mock.SetupGet(_ => _.HttpContext.Request.QueryString)
                    .Returns(new NameValueCollection
                {
                    { "fkName", "UserId" },
                    { "fkValue", "2" }
                });
                mock.SetupGet(_ => _.HttpContext.User.Identity.IsAuthenticated)
                    .Returns(true);
                mock.SetupGet(m => m.RouteData)
                    .Returns(routeData);
                engine.Setup(e => e.FindPartialView(It.IsAny<ControllerContext>(), It.IsAny<string>(), It.IsAny<bool>()))
                    .Returns(viewEngineResult);

                ViewEngines.Engines.Clear();
                ViewEngines.Engines.Add(engine.Object);
                ViewData = new ViewDataDictionary();
                ControllerContext = mock.Object;
            }
        }

        readonly Mock<IRepository<Person>> _repository = new Mock<IRepository<Person>>();
        private readonly MockController _controller;
        private readonly Mock<IControllerPackage<Person>> _package = new Mock<IControllerPackage<Person>>();

        public BaseControllerTests()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("gb");
            _controller = new MockController(_package.Object);

            _package.SetupGet(_ => _.Repository)
                .Returns(_repository.Object);
            _repository.Setup(_ => _.GetName(It.IsAny<string>(), It.IsAny<int>()))
                .Returns("Gizzie");
        }

        [Fact]
        public void Index_ShouldSetTitle_AndSubTitle()
        {
            _controller.Index();

            Assert.Equal("Persons", _controller.ViewBag.Title);
            Assert.Equal("Persons for Gizzie", _controller.ViewBag.SubTitle);
        }

    }

}
