using System.Text;
using System.Web.Mvc;
using K9.SharedLibrary.Helpers;
using K9.WebApplication.Constants.Html;
using HtmlHelper = System.Web.Mvc.HtmlHelper;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString ImageLoader(this HtmlHelper html, string imagePath)
		{
			var images = ContentHelper.GetImageFiles(imagePath);

			var divBuilder = new TagBuilder(Tags.Div);
			divBuilder.MergeAttribute(Attributes.Id, "imagePreloader");
			divBuilder.MergeAttribute(Attributes.Style, "position: absolute; left: -9999px; top: -9999px;");

			var sb = new StringBuilder();
			foreach (var image in images)
			{
				var imgBuilder = new TagBuilder(Tags.Image);
				imgBuilder.MergeAttribute(Attributes.Src, image.Src);
				sb.AppendLine(imgBuilder.ToString(TagRenderMode.SelfClosing));
			}

			divBuilder.InnerHtml = sb.ToString();
			return MvcHtmlString.Create(divBuilder.ToString());
		}

	}
}