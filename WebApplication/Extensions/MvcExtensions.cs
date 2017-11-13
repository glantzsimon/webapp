using K9.Base.Globalisation;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace K9.Base.WebApplication.Extensions
{
    public static class MvcExtensions
	{

		public static string GetActiveClass(this ViewContext viewContext, string actionName, string controllerName)
		{
			return viewContext.RouteData.Values["action"].ToString().ToLower() == actionName.ToLower() &&
				viewContext.RouteData.Values["controller"].ToString().ToLower() == controllerName.ToLower() ? "active" : "";
		}

		public static void AddErrorMessageFromException<T>(this ModelStateDictionary modelState, Exception ex, T item) where T : IObjectBase
		{
			var duplicateIndexErrorPropertyName = ex.GetDuplicateIndexErrorPropertyName();
			if (!string.IsNullOrEmpty(duplicateIndexErrorPropertyName))
			{
				var classType = typeof(T);
				var columnInfo = typeof(T).GetProperties().First(c => c.Name == duplicateIndexErrorPropertyName);
				modelState.AddModelError(duplicateIndexErrorPropertyName, string.Format(
					Dictionary.DuplicateIndexError,
					classType.GetIndefiniteArticle(),
					classType.GetName().ToLower(),
					columnInfo.GetDefiniteArticle().ToLower(),
					columnInfo.GetDisplayName(),
					item.GetProperty(duplicateIndexErrorPropertyName)));
			}
			else if (ex.IsDeleteConflictError())
			{
				modelState.AddModelError("", Dictionary.DeleteConflictError);
			}
			else
			{
				modelState.AddModelError("", Dictionary.FriendlyErrorMessage);
			}
		}

		public static IDataSetsHelper GetDropdownData(this WebViewPage view)
		{
			var baseController = view.ViewContext.Controller as IBaseController;
			return baseController?.DropdownDataSets;
		}

		public static IRoles GetRoles(this WebViewPage view)
		{
			var baseController = view.ViewContext.Controller as IBaseController;
			return baseController?.Roles;
		}

		public static string GetControllerName(this Type type)
		{
			return type.Name.Replace("Controller", string.Empty);
		}

	    public static IAuthentication GetAuthentication(this HtmlHelper helper)
        {
	        var controller = helper.ViewContext.Controller as IBaseController;
	        return controller?.Authentication;
	    }

	}
}