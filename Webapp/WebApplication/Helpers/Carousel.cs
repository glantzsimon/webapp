using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString Carousel(this HtmlHelper helper, CarouselOptions options)
		{
			return helper.Partial("controls/_Carousel", options);
		}

	}
}