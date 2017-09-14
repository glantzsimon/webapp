using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Options
{
	public interface IDataTableOptions
	{
		string TableId { get; }
		string Action { get; set; }
		string Controller { get; set; }
		IColumnsConfig ColumnsConfig { get; set; }
		IStatelessFilter StatelessFilter { get; set; }
		string GetDataUrl(UrlHelper geturlHeler);
		bool AllowCreate { get; set; }
		bool AllowEdit { get; set; }
		bool AllowDelete { get; set; }
		bool AllowView { get; set; }
		bool DisplayFooter { get; set; }
		List<IButton> CustomButtons { get; set; }
		bool AllowCrud();
		List<PropertyInfo> GetColumns();
		List<string> GetColumnNames();
		MvcHtmlString GetColumnsJson();
		MvcHtmlString GetColumnDefsJson();
		string GetButtonRenderFunction();
		List<DataTableColumnInfo> GetColumnInfos();
		RouteValueDictionary GetFilterRouteValues();
		string GetQueryStringJoiner();
	}
}