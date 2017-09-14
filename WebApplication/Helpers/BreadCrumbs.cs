using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Models;
using K9.Base.WebApplication.Options;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BreadCrumbs(this HtmlHelper html)
		{
			var crumbs = (html.ViewBag.Crumbs != null) ? html.ViewBag.Crumbs as List<Crumb> : new List<Crumb>();
			return html.Partial("_BreadCrumbs", new BreadCrumbsOptions(crumbs, html.ViewContext));
		}

	}
}