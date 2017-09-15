using System;
using System.Reflection;
using K9.Base.DataAccessLayer.Attributes;
using K9.Globalisation;
using K9.SharedLibrary.Extensions;

namespace K9.Base.WebApplication.Extensions
{
	public static class Extensions
	{

		public static string GetTableName(this Type type)
		{
			return $"{type.Name}Table";
		}

		public static string GetDataTableType(this PropertyInfo property)
		{
			if (property.PropertyType == typeof(int))
			{
				return "num";
			}
			if (property.PropertyType == typeof(DateTime))
			{
				return "date";
			}

			return "string";
		}

		public static string GetDefiniteArticle(this Type type)
		{
			var attribute = type.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : attribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this Type type)
		{
			var attribute = type.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : attribute.GetIndefiniteArticle();
		}

		public static string GetDefiniteArticle(this PropertyInfo info)
		{
			var attribute = info.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : attribute.GetDefiniteArticle();
		}

		public static string GetIndefiniteArticle(this PropertyInfo info)
		{
			var attribute = info.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.MasculineDefiniteArticle) : attribute.GetIndefiniteArticle();
		}

		public static string GetOfPreposition(this PropertyInfo info)
		{
			var attribute = info.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.OfPreposition) : attribute.GetOfPreposition();
		}

		public static string GetOfPreposition(this Type type)
		{
			var attribute = type.GetAttribute<GrammarAttribute>();
			return attribute == null ? typeof(Dictionary).GetValueFromResource(Strings.Grammar.OfPreposition) : attribute.GetOfPreposition();
		}

		public static string GetName(this Type type)
		{
			var namettribute = type.GetAttribute<NameAttribute>();
			return namettribute == null ? type.Name : namettribute.GetName();
		}

		public static string GetPluralName(this Type type)
		{
			var namettribute = type.GetAttribute<NameAttribute>();
			return namettribute == null ? $"{type.Name}s" : namettribute.GetPluralName();
		}

		public static string GetListName(this Type type)
		{
			var namettribute = type.GetAttribute<NameAttribute>();
			return namettribute == null ? $"{type.Name}s" : namettribute.GetListName();
		}

	}
}