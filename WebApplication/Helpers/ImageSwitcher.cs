using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlExtensions
	{
		
		public static MvcHtmlString ImageSwitcher(this HtmlHelper helper, ImageSwitcherOptions options)
		{
			return helper.Partial("controls/_ImageSwitcher", options);
		}

		public static MvcHtmlString BackgroundImageSwitcher(this HtmlHelper helper, BackgroundImageSwitcherOptions options)
		{
			return helper.Partial("controls/_BackgroundImageSwitcher", options);
		}

	}
}