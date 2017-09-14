using System;
using System.Web.Mvc;
using K9.WebApplication.Constants;
using K9.WebApplication.Constants.Html;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static IDisposable BeginBootstrapForm(this HtmlHelper html, string title = "", string titleTag = Tags.H2)
		{
			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, Bootstrap.Classes.Well);
			html.ViewContext.Writer.WriteLine(div.ToString(TagRenderMode.StartTag));

			html.ViewContext.Writer.WriteLine(html.AntiForgeryToken());
			html.ViewContext.Writer.WriteLine(html.BootstrapValidationSummary());

			if (!string.IsNullOrEmpty(title))
			{
				var h = new TagBuilder(titleTag);
				h.SetInnerText(title);
				html.ViewContext.Writer.WriteLine(h.ToString());
			}

			return new TagCloser(html, Tags.Div);
		}

	}
}