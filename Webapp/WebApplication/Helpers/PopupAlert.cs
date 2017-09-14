using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.WebApplication.Enums;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString PopupAlert(this HtmlHelper html, EAlertType alertType, string message, MvcHtmlString otherMessage)
		{
			return PopupAlert(html, alertType, message, otherMessage.ToString());
		}

		public static MvcHtmlString PopupAlert(this HtmlHelper html, EAlertType alertType, MvcHtmlString message, MvcHtmlString otherMessage)
		{
			return PopupAlert(html, alertType, message.ToString(), otherMessage.ToString());
		}

		public static MvcHtmlString PopupAlert(this HtmlHelper html, EAlertType alertType, MvcHtmlString message)
		{
			return PopupAlert(html, alertType, message.ToString(), String.Empty);
		}

		public static MvcHtmlString PopupAlert(this HtmlHelper html, EAlertType alertType, string message, string otherMessage = "")
		{
			return PopupAlert(html, new AlertOptions
			{
				Message = message,
				OtherMessage = otherMessage,
				AlertType = alertType
			});
		}

		public static MvcHtmlString PopupAlert(this HtmlHelper html)
		{
			if (html.ViewBag.IsPopupAlert != null && (bool)html.ViewBag.IsPopupAlert)
			{
				return html.Partial("_PopupAlert", html.ViewBag.AlertOptions as AlertOptions);
			}
			return MvcHtmlString.Empty;
		}

		public static MvcHtmlString PopupAlert(this HtmlHelper html, AlertOptions options)
		{
			return html.Partial("_PopupAlert", options);
		}

	}
}