using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using K9.Base.WebApplication.Exceptions;
using K9.Base.WebApplication.Extensions;
using K9.Base.WebApplication.Helpers;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Options
{
	public class DataTableOptions<T> : IDataTableOptions where T : IObjectBase
	{
		private List<PropertyInfo> _columns;
		private List<DataTableColumnInfo> _columnInfos;

		public string Action { get; set; }
		public string Controller { get; set; }
		public bool DisplayFooter { get; set; }
		public List<IButton> CustomButtons { get; set; }
		public List<string> VisibleColumns { get; set; }
		public IColumnsConfig ColumnsConfig { get; set; }
		public IStatelessFilter StatelessFilter { get; set; }
		public bool AllowCreate { get; set; }
		public bool AllowEdit { get; set; }
		public bool AllowDelete { get; set; }
		public bool AllowView { get; set; }

		public string TableId => $"{typeof(T).Name}Table";

	    public DataTableOptions()
		{
			VisibleColumns = new List<string>();
			CustomButtons = new List<IButton>();
			AllowCreate = true;
			AllowDelete = true;
			AllowView = true;
			AllowEdit = true;
		}

		public bool AllowCrud()
		{
			return AllowCreate || AllowEdit || AllowDelete || AllowView || CustomButtons.Any();
		}

		public List<string> GetColumnNames()
		{
			return GetColumns().Select(c => c.Name).ToList();
		}

		public string GetDataUrl(UrlHelper urlHeler)
		{
			var actionName = string.IsNullOrEmpty(Action) ? "List" : Action;
			var controllerName = string.IsNullOrEmpty(Controller) ? typeof(T).GetListName() : Controller;
			return urlHeler.Action(actionName, controllerName, GetFilterRouteValues());
		}

		public List<PropertyInfo> GetColumns()
		{
			if (_columns == null)
			{
				var allColumns = typeof(T).GetProperties()
					.Where(p => !ColumnsConfig.ColumnsToIgnore.Contains(p.Name)).ToList();
				
				if (VisibleColumns.Any())
				{
					var invalidColumns = VisibleColumns.Where(v => !allColumns.Select(c => c.Name).Contains(v));
					if (invalidColumns.Any())
					{
						throw new InvalidColumnNameException(invalidColumns.ToDelimitedString());
					}
				}

				var orderedColumns = VisibleColumns.Select(visibleColumn => allColumns.FirstOrDefault(c => c.Name == visibleColumn)).ToList();
				orderedColumns.AddRange(allColumns.Where(c => !c.IsVirtual() && !orderedColumns.Contains(c)));

				_columns = new List<PropertyInfo>();
				_columns.AddRange(orderedColumns);
			}
		    return _columns;
		}

		public MvcHtmlString GetColumnsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(GetColumnInfos().Select(c => new
			{
				data = c.Data,
				title = c.Name,
				orderable = true
			}).ToArray()));
		}

		public MvcHtmlString GetColumnDefsJson()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(
				GetColumnInfos().Select(c => new
				{
					targets = new[] { c.Index },
					visible = c.IsVisible,
					dataType = c.DataType
				})));
		}

		public string GetButtonRenderFunction()
		{
			return $"renderButtons{TableId}";
		}

		public List<DataTableColumnInfo> GetColumnInfos()
		{
			if (_columnInfos == null)
			{
				_columnInfos = new List<DataTableColumnInfo>();

				var keyColumns = GetKeyColumns();
				var columnsInfos = GetColumns().Select((c, index) =>
				{
					var info = new DataTableColumnInfo(index)
					{
						IsDatabound = c.IsDataBound(),
						IsVisible = !keyColumns.Select(k => k.Name).Contains(c.Name) && (!VisibleColumns.Any() || VisibleColumns.Contains(c.Name)),
						DataType = c.PropertyType.ToString().ToLower()
					};
					info.UpdateData(c.Name);
					info.UpdateName(c.GetDisplayName());
					return info;
				}).ToList();

				_columnInfos.AddRange(columnsInfos);
			}
			return _columnInfos;
		}

		public RouteValueDictionary GetFilterRouteValues()
		{
			if (StatelessFilter != null)
			{
				return StatelessFilter.GetFilterRouteValues();
			}
			return null;
		}

		public string GetQueryStringJoiner()
		{
			var routeValues = GetFilterRouteValues();
			return routeValues != null && routeValues.Any() ? "&" : "?";
		}

		private List<PropertyInfo> GetKeyColumns()
		{
			return GetColumns().GetPropertiesWithAttributes(typeof(KeyAttribute), typeof(ForeignKeyAttribute));
		}

	}
}