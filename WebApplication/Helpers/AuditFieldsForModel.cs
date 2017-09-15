using System.Text;
using System.Web.Mvc;
using K9.Base.DataAccessLayer.Models;
using K9.Globalisation;
using K9.SharedLibrary.Authentication;
using WebMatrix.WebData;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString AuditFieldsForModel<T>(this HtmlHelper<T> html, T model) where T : ObjectBase
		{
			if (System.Web.Security.Roles.IsUserInRole(WebSecurity.CurrentUserName, RoleNames.Administrators))
			{
				var sb = new StringBuilder();

				sb.AppendLine(html.SubTitle(Dictionary.AuditInformation).ToString());

				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.CreatedBy).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedOn).ToString());
				sb.AppendLine(html.BootstrapDisplayFor(m => model.LastUpdatedBy).ToString());

				return MvcHtmlString.Create(sb.ToString());
			}

			return MvcHtmlString.Empty;
		}

	}
}