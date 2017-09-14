using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static string GetDisplayNameFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			return metaData.DisplayName ?? (metaData.PropertyName.SplitOnCapitalLetter() ?? ExpressionHelper.GetExpressionText(expression));
		}

		public static string GetPropertyNamesFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			var metaData = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
			return metaData.PropertyName ?? ExpressionHelper.GetExpressionText(expression);
		}

		public static List<ModelError> GetModelErrors(this HtmlHelper html, bool excludePropertyErrors = false)
		{
			return html.ViewData.ModelState.ToList().Where(x => !excludePropertyErrors || string.IsNullOrEmpty(x.Key)).SelectMany(x => x.Value.Errors).ToList();
		}

		public static List<ModelError> GetModelErrorsFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
		{
			return html.ViewData.ModelState.Where(x => x.Key == html.GetPropertyNamesFor(expression)).SelectMany(x => x.Value.Errors).ToList();
		}

		public static UrlHelper GeturlHeler(this HtmlHelper html)
		{
			return new UrlHelper(html.ViewContext.RequestContext);
		}

		public static IRoles GetRoles(this HtmlHelper html)
		{
			var baseController = html.ViewContext.Controller as IBaseController;
			return baseController.Roles;
		}

	}
}