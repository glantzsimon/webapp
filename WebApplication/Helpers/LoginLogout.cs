using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Constants.Html;
using K9.Base.WebApplication.Extensions;
using K9.Base.Globalisation;
using WebMatrix.WebData;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString LoginLogout(this HtmlHelper html)
		{
			var sb = new StringBuilder();

			if (WebSecurity.IsAuthenticated)
			{
				var icon = new TagBuilder(Tags.Icon);
				icon.MergeAttribute(Attributes.Class, "fa fa-sign-out");

				var anchor = new TagBuilder(Tags.Anchor);
				anchor.MergeAttribute(Attributes.Href, html.GeturlHeler().Action("LogOff", "Account"));
				anchor.InnerHtml = $"{icon} {Dictionary.LogOut}";

				var li = new TagBuilder(Tags.Li) { InnerHtml = anchor.ToString() };
				li.MergeAttribute(Attributes.Class, html.ViewContext.GetActiveClass("LogOff", "Account"));
				sb.Append(li);
			}
			else
			{
				var icon = new TagBuilder(Tags.Icon);
				icon.MergeAttribute(Attributes.Class, "fa fa-sign-in");

				var anchor = new TagBuilder(Tags.Anchor);
				anchor.MergeAttribute(Attributes.Href, html.GeturlHeler().Action("Login", "Account"));
				anchor.InnerHtml = $"{icon} {Dictionary.LogIn}";

				var li = new TagBuilder(Tags.Li) { InnerHtml = anchor.ToString() };
				li.MergeAttribute(Attributes.Class, html.ViewContext.GetActiveClass("Login", "Account"));
				sb.Append(li);

				li = new TagBuilder(Tags.Li) { InnerHtml = html.ActionLink(Dictionary.Register, "Register", "Account").ToString() };
				li.MergeAttribute(Attributes.Class, html.ViewContext.GetActiveClass("Register", "Account"));
				sb.Append(li);
			}

			return MvcHtmlString.Create(sb.ToString());
		}

	}
}