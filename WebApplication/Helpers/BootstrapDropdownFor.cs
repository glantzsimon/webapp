using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Constants;
using K9.Globalisation;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapDropdownFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, SelectList selectList, bool liveSearch = false)
		{
			return html.DropDownListFor(expression, selectList, null, new { @class =
			    $"{Bootstrap.Classes.SelectList} {Bootstrap.Classes.FormControl}", title = Dictionary.PleaseSelect, data_live_search = liveSearch });
		}

	}
}