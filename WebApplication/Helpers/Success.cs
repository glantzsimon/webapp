﻿using System;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Options;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString Success(this HtmlHelper html, string message, MvcHtmlString otherMessage)
		{
			return Success(html, message, otherMessage.ToString());
		}

		public static MvcHtmlString Success(this HtmlHelper html, MvcHtmlString message, MvcHtmlString otherMessage)
		{
			return Success(html, message.ToString(), otherMessage.ToString());
		}

		public static MvcHtmlString Success(this HtmlHelper html, MvcHtmlString message)
		{
			return Success(html, message.ToString(), String.Empty);
		}

		public static MvcHtmlString Success(this HtmlHelper html, string message, string otherMessage = "")
		{
			return html.Partial("_Alert", new AlertOptions
			{
				Message = message,
				OtherMessage = otherMessage,
				AlertType = EAlertType.Success
			});
		}

	}
}