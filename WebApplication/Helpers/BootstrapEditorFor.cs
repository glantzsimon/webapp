using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using K9.Base.WebApplication.Constants;
using K9.Base.WebApplication.Constants.Html;
using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Options;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Helpers
{
	public static partial class HtmlHelpers
	{

		public static MvcHtmlString BootstrapEditorFor<TModel, TProperty>(this HtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, EditorOptions options = null)
		{
			var sb = new StringBuilder();
			var modelType = ModelMetadata.FromLambdaExpression(expression, html.ViewData).ModelType;

			// Get additional view data for the control
			var viewDataDictionary = new ViewDataDictionary();
			options = options ?? new EditorOptions();
			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputSize.ToCssClass());
			viewDataDictionary.MergeAttribute(Attributes.Class, options.InputWidth.ToCssClass());

			if (modelType != typeof(bool))
			{
				viewDataDictionary.MergeAttribute(Attributes.Class, Bootstrap.Classes.FormControl);
			}

			viewDataDictionary.MergeAttribute(Attributes.PlaceHolder, options.PlaceHolder);
			viewDataDictionary.MergeAttribute(Attributes.Title, string.Empty);

			// Get container div
			var div = new TagBuilder(Tags.Div);
			var attributes = new Dictionary<string, object>();
			attributes.MergeAttribute(Attributes.Class, !options.IsReadOnly && modelType == typeof(bool) ? Bootstrap.Classes.Checkbox : Bootstrap.Classes.FormGroup);
			if (html.GetModelErrorsFor(expression).Any())
			{
				attributes.MergeAttribute(Attributes.Class, Bootstrap.Classes.HasError);
			}

			div.MergeAttributes(attributes);
			sb.AppendLine(div.ToString(TagRenderMode.StartTag));

			var hideLabelForTypes = new List<Type> { typeof(bool), typeof(FileSource) };
			if (!hideLabelForTypes.Contains(modelType))
			{
				sb.AppendLine(html.LabelFor(expression, options.Label).ToString());
			}

			if (options.IsReadOnly)
			{
				sb.AppendLine(html.DisplayFor(expression, new { viewDataDictionary }).ToString());
			}
			else
			{
				sb.AppendLine(html.EditorFor(expression, new { viewDataDictionary }).ToString());
				sb.AppendLine(html.ValidationMessageFor(expression).ToString());
			}
			sb.AppendLine(div.ToString(TagRenderMode.EndTag));

			return MvcHtmlString.Create(sb.ToString());
		}

		public static MvcHtmlString BootstrapDisplayFor<TModel, TProperty>(this HtmlHelper<TModel> html,
			Expression<Func<TModel, TProperty>> expression, EditorOptions options = null)
		{
			return BootstrapEditorFor(html, expression, new EditorOptions
			{
				IsReadOnly = true
			});
		}

	}
}