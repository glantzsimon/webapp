using Moq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace K9.WebApplication.Tests.Shared
{
    public static class HtmlHelper
    {
        public static TestHtmlHelper<TModel> CreateHtmlHelper<TModel>(TModel model) where TModel : class
        {
            var viewDataDictionary = new ViewDataDictionary(model);

            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream);

            var mockViewContext = new Mock<ViewContext>(
                new ControllerContext(
                    new Mock<HttpContextBase>().Object,
                    new RouteData(),
                    new Mock<ControllerBase>().Object
                ),
                new Mock<IView>().Object,
                viewDataDictionary,
                new TempDataDictionary(),
                streamWriter
            );

            mockViewContext.Setup(vc => vc.Writer).Returns(streamWriter);

            var mockDataContainer = new Mock<IViewDataContainer>();
            mockDataContainer.Setup(c => c.ViewData).Returns(viewDataDictionary);

            return new TestHtmlHelper<TModel>(mockViewContext.Object, mockDataContainer.Object, stream, streamWriter);
        }
    }

    public class TestHtmlHelper<TModel> : HtmlHelper<TModel> where TModel : class
    {
        private readonly MemoryStream _stream;
        private readonly StreamWriter _streamWriter;

        public TestHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, MemoryStream stream, StreamWriter streamWriter)
            : base(viewContext, viewDataContainer)
        {
            _stream = stream;
            _streamWriter = streamWriter;
        }

        public TestHtmlHelper(ViewContext viewContext, IViewDataContainer viewDataContainer, RouteCollection routeCollection, MemoryStream stream, StreamWriter streamWriter)
            : base(viewContext, viewDataContainer, routeCollection)
        {
            _stream = stream;
            _streamWriter = streamWriter;
        }

        public string GetOutputFromStream()
        {
            _streamWriter.Flush();
            var result = Encoding.UTF8.GetString(_stream.ToArray());

            _stream.Close();
            _streamWriter.Close();

            return result;
        }

    }
}
