using System;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Constants;
using K9.Base.Globalisation;
using K9.SharedLibrary.Extensions;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapDropdownFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, SelectList selectList, bool liveSearch = false, ViewDataDictionary htmlAttributes = null)
		{
            htmlAttributes = htmlAttributes ?? new ViewDataDictionary();
		    htmlAttributes.MergeAttribute("class", Bootstrap.Classes.SelectList);
		    htmlAttributes.MergeAttribute("class", Bootstrap.Classes.FormControl);
		    htmlAttributes.MergeAttribute("title", Dictionary.PleaseSelect);
		    htmlAttributes.MergeAttribute("data-live-search", liveSearch);
            return html.DropDownListFor(expression, selectList, null, htmlAttributes);
		}

	}
}