using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Options;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString Alert(this HtmlHelper html, EAlertType alertType, string message, MvcHtmlString otherMessage)
		{
			return Alert(html, alertType,message, otherMessage.ToString());
		}

		public static MvcHtmlString Alert(this HtmlHelper html, EAlertType alertType, MvcHtmlString message, MvcHtmlString otherMessage)
		{
			return Alert(html, alertType, message.ToString(), otherMessage.ToString());
		}

		public static MvcHtmlString Alert(this HtmlHelper html, EAlertType alertType, MvcHtmlString message)
		{
			return Alert(html, alertType, message.ToString(), String.Empty);
		}

		public static MvcHtmlString Alert(this HtmlHelper html, EAlertType alertType, string message, string otherMessage = "")
		{
			return html.Partial("_Alert", new AlertOptions
			{
				Message = message,
				OtherMessage = otherMessage,
				AlertType = alertType
			});
		}

	}
}