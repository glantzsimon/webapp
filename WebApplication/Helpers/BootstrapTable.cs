using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Constants.Html;
using K9.WebApplication.Extensions;
using K9.WebApplication.Options;

namespace K9.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		private static IColumnsConfig _columnsConfig;

		public static void SetIgnoreColumns(IColumnsConfig columnsConfig)
		{
			_columnsConfig = columnsConfig;
		}

		public static MvcHtmlString BootstrapTable<T>(this HtmlHelper<T> html, IDataTableOptions options = null) where T : IObjectBase
		{
			return GetBootstrapTable<T>(html, options);
		}

		public static MvcHtmlString BootstrapTable<T>(this HtmlHelper html, IDataTableOptions options = null) where T : IObjectBase
		{
			return GetBootstrapTable<T>(html, options);
		}

		private static MvcHtmlString GetBootstrapTable<T>(this HtmlHelper html, IDataTableOptions options = null) where T : IObjectBase
		{
			options = options ?? new DataTableOptions<T>();
			options.ColumnsConfig = _columnsConfig;
			options.StatelessFilter = html.ViewContext.HttpContext.Request.GetStatelessFilter();

			var roles = html.GetRoles();
			options.AllowCreate = options.AllowCreate && (roles.CurrentUserHasPermissions<T>(Permissions.Create) || roles.CurrentUserIsInRoles(RoleNames.Administrators));
			options.AllowDelete = options.AllowDelete && (roles.CurrentUserHasPermissions<T>(Permissions.Delete) || roles.CurrentUserIsInRoles(RoleNames.Administrators));
			options.AllowView = options.AllowView && (roles.CurrentUserHasPermissions<T>(Permissions.View) || roles.CurrentUserIsInRoles(RoleNames.Administrators));
			options.AllowEdit = options.AllowEdit && (roles.CurrentUserHasPermissions<T>(Permissions.Edit) || roles.CurrentUserIsInRoles(RoleNames.Administrators));

			var modelType = typeof(T);
			var sb = new StringBuilder();

			if (options.AllowCreate)
			{
				var createDiv = new TagBuilder(Tags.Div);
				createDiv.MergeAttribute(Attributes.Class, "button-container");
				createDiv.InnerHtml = html.BootstrapCreateNewButton().ToString();
				sb.Append(createDiv);
			}

			// Create table container
			var div = new TagBuilder(Tags.Div);
			div.MergeAttribute(Attributes.Class, "datatable-container");

			// Create table
			var table = new TagBuilder(Tags.Table);
			var tableName = modelType.GetTableName();
			var dataUrl = options.GetDataUrl(html.GeturlHeler());
			table.MergeAttribute(Attributes.Id, tableName);
			table.MergeAttribute(Attributes.Class, "bootstraptable table table-striped table-bordered");
			table.MergeAttribute(Attributes.CellSpacing, "0");
			table.MergeAttribute(Attributes.Width, "100%");
			table.MergeAttribute(Attributes.DataUrl, dataUrl);

			// Add header
			var thead = new TagBuilder(Tags.Thead);
			options.GetColumnNames();
			thead.AddColumns(options);
			table.InnerHtml += thead.ToString();

			// Add footer
			if (options.DisplayFooter)
			{
				var tfoot = new TagBuilder(Tags.TFoot);
				tfoot.AddColumns(options);
				table.InnerHtml += tfoot.ToString();
			}

			div.InnerHtml += table.ToString();

			sb.Append(div);
			sb.AppendLine(html.Partial("Controls/_DataTablesJs", options).ToString());

			return MvcHtmlString.Create(sb.ToString());
		}

		private static void AddColumns(this TagBuilder builder, IDataTableOptions options)
		{
			var tr = new TagBuilder(Tags.Tr);
			foreach (var column in options.GetColumnNames())
			{
				var th = new TagBuilder(Tags.Th);
				th.SetInnerText(column);
				tr.InnerHtml += th.ToString();
			}

			if (options.AllowCrud())
			{
				var th = new TagBuilder(Tags.Th);
				tr.InnerHtml += th.ToString();
			}

			builder.InnerHtml += tr.ToString();
		}

	}
}