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
using Xunit;

namespace K9.WebApplication.Tests.Unit.Controllers
{
    public class BaseControllerTests
    {
        public static readonly Mock<HttpResponseBase> MockHttpResponse = new Mock<HttpResponseBase>();

        public class MockController : BaseController<Person>
        {
            public StreamWriter TextWriter { get; }

            public MockController(IControllerPackage<Person> controllerPackage) : base(controllerPackage)
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

        [Fact]
        public void Details_ShouldReturnHttpNotFoundIfItemNotFound()
        {
            var id = 2;
            var statuscode = 0;

            _repository.Setup(_ => _.Find(id))
                .Returns((Person)null);
            MockHttpResponse.SetupSet(_ => _.StatusCode = It.IsAny<int>())
                .Callback<int>(value => statuscode = value);
            
            var view = Assert.IsType<ViewResult>(_controller.Details(id));
            Assert.Equal("NotFound", view.ViewName);
            Assert.Equal((int)HttpStatusCode.NotFound, statuscode);
        }

    }

}
