using System;
using System.Web.Mvc;

namespace K9.WebApplication.Helpers
{
	public class TagCloser : IDisposable
	{
		private readonly HtmlHelper _html;
		private readonly string[] _tagNames;

		public TagCloser(HtmlHelper html, params string[] tagNames)
		{
			_html = html;
			_tagNames = tagNames;
		}

		public TagCloser(HtmlHelper html, string tagName)
		{
			_html = html;
			_tagNames = new[] { tagName };
		}

		public void Dispose()
		{
			foreach (var tag in _tagNames)
			{
				var tb = new TagBuilder(tag);
				_html.ViewContext.Writer.WriteLine(tb.ToString(TagRenderMode.EndTag));
			}
		}
	}
}