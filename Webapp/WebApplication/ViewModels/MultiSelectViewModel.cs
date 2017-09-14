
using System.Collections.Generic;
using System.Linq;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Extensions;
using K9.WebApplication.Models;

namespace K9.WebApplication.ViewModels
{
	public class MultiSelectViewModel
	{
		public int ParentId { get; set; }
		public string ParentTypeName { get; set; }
		public string ParentDescription { get; set; }
		public List<MultiSelectItem> Items { get; set; }

		public string GetSubTitle()
		{
			return $"{ParentTypeName}: {ParentDescription}";
		}

		public static MultiSelectViewModel Create<T, T2, T3>(T2 parent, List<T> items) 
			where T : class, IObjectBase
			where T2 : class, IObjectBase
			where T3 : class, IObjectBase
		{
			return new MultiSelectViewModel
			{
				ParentId = parent.Id,
				ParentTypeName = typeof(T2).GetName(),
				ParentDescription = parent.Description,
				Items = items.Select(item =>
				{
					var child = item.GetProperty(typeof (T3).Name) as T3;
					return new MultiSelectItem
					{
						Id = item.Id,
						ChildId = (int) item.GetProperty(typeof (T3).GetForeignKeyName()),
						Description = child.Description,
						IsSelected = item.Id > 0
					};
				}).ToList()
			};
		}
	}
}