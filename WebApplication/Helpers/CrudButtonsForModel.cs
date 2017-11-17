using System.Text;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Attributes;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Helpers
{
    public static partial class HtmlHelpers
    {

        public static MvcHtmlString CrudButtonsForModel<T>(this HtmlHelper<T> html, T model) where T : IObjectBase
        {
            var sb = new StringBuilder();
            var roles = html.GetRoles();

            if ((roles.CurrentUserIsInRoles(RoleNames.Administrators) || roles.CurrentUserHasPermissions<T>(Permissions.Edit)) && !model.IsSystemStandard)
            {
                sb.AppendLine(html.BootstrapLinkToEditButton(model.Id).ToString());
            }

            var noDelete = (typeof(T).HasAttribute(typeof(SoftDeleteAttribute))) && model.IsDeleted;
            if ((roles.CurrentUserIsInRoles(RoleNames.Administrators) || roles.CurrentUserHasPermissions<T>(Permissions.Delete)) && !model.IsSystemStandard && !noDelete)
            {
                sb.AppendLine(html.BootstrapLinkToDeleteButton(model.Id).ToString());
            }
            return MvcHtmlString.Create(sb.ToString());
        }

    }
}