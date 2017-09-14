using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapValidationSummary(this HtmlHelper html)
		{
			var errors = html.GetModelErrors(true);
			if (errors.Any())
			{
				return html.Partial("_ValidationSummary", errors);
			}
			
			return MvcHtmlString.Empty;
		}

	}
}