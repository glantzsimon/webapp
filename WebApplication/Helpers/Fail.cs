using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Enums;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString Failure(this HtmlHelper html, string message, MvcHtmlString otherMessage)
		{
			return Failure(html, message, otherMessage.ToString());
		}

		public static MvcHtmlString Failure(this HtmlHelper html, MvcHtmlString message, MvcHtmlString otherMessage)
		{
			return Failure(html, message.ToString(), otherMessage.ToString());
		}

		public static MvcHtmlString Failure(this HtmlHelper html, MvcHtmlString message)
		{
			return Failure(html, message.ToString(), string.Empty);
		}

		public static MvcHtmlString Failure(this HtmlHelper html, string message, string otherMessage = "")
		{
			return html.Partial("_Alert", new AlertOptions
			{
				Message = message,
				OtherMessage = otherMessage,
				AlertType = EAlertType.Fail
			});
		}

	}
}