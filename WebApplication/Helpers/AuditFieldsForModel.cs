using K9.Base.DataAccessLayer.Attributes;
using K9.Base.DataAccessLayer.Models;
using K9.Base.Globalisation;
using K9.SharedLibrary.Authentication;
using K9.SharedLibrary.Extensions;
using System.Text;
using System.Web.Mvc;

namespace K9.Base.WebApplication.Helpers
{
    public static partial class HtmlHelpers
	{

		public static MvcHtmlString AuditFieldsForModel<T>(this HtmlHelper<T> html, T model) where T : ObjectBase
		{
		    var roles = html.GetRoles();
			if (roles.CurrentUserIsInRoles(RoleNames.Administrators))
			{
				var sb = new StringBuilder();

				sb.AppendLine(html.SubTitle(Dictionary.AuditInformation).ToString());

				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedBy).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedBy).ToString());
			    if (typeof(T).HasAttribute(typeof(SoftDeleteAttribute)))
			    {
			        sb.AppendLine(html.BootstrapDisplayFor(m => model.IsDeleted).ToString());
                }

                    return MvcHtmlString.Create(sb.ToString());
			}

			return MvcHtmlString.Empty;
		}

	}
}