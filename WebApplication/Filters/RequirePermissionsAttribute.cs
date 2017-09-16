using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Models;
using WebMatrix.WebData;

namespace K9.Base.WebApplication.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method )]
	[DefaultProperty("Permission")]
	public class RequirePermissionsAttribute : ActionFilterAttribute
	{

		public string Permission { get; set; }
		public string Role { get; set; }
        
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var controller = filterContext.Controller as IBaseController;
			var roles = controller?.Roles;
		    var authentication = controller?.Authentication;
            
			// Check controller level roles first
		    if (controller?.GetType().GetCustomAttributes(typeof(RequirePermissionsAttribute), true).FirstOrDefault() is RequirePermissionsAttribute controllerPermissionAttribute)
			{
				if (!string.IsNullOrEmpty(controllerPermissionAttribute.Role))
				{
					if (!CheckRole(roles, controllerPermissionAttribute.Role, authentication))
					{
						HttpForbidden(filterContext);
						return;
					}
				}
			}

			if (!string.IsNullOrEmpty(Permission))
			{
				var fullyQualifiedPermissionName = $"{Permission}{controller.GetObjectName()}";
				if (!CheckPermission(roles, fullyQualifiedPermissionName, authentication))
				{
					HttpForbidden(filterContext);
					return;
				}
			}

			if (!string.IsNullOrEmpty(Role))
			{
				if (!CheckRole(roles, Role, authentication))
				{
					HttpForbidden(filterContext);
				}
			}
		}

		private void HttpForbidden(ActionExecutingContext filterContext)
		{
			var unauthorized = "Unauthorized";
			filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			filterContext.Result = new ViewResult
			{
				ViewName = unauthorized
			};
		}

		private bool CheckPermission(IRoles roles, string permissionName, IAuthentication authentication)
		{
			var permissions = roles.GetPermissionsForCurrentUser().Select(r => r.Name).ToList();
			if ((!authentication.IsAuthenticated || !permissions.Contains(permissionName)) && !roles.CurrentUserIsInRoles(RoleNames.Administrators))
			{
				return false;
			}
			return true;
		}

		private bool CheckRole(IRoles roles, string roleName, IAuthentication authentication)
		{
			var userRoles = roles.GetRolesForCurrentUser().Select(r => r.Name).ToList();
			if ((!authentication.IsAuthenticated || !userRoles.Contains(roleName)) && !roles.CurrentUserIsInRoles(RoleNames.Administrators))
			{
				return false;
			}

			return true;
		}


	}
}