
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using K9.DataAccessLayer.Extensions;
using K9.DataAccessLayer.Respositories;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.DataSets
{

	public class DataSetsHelper : IDataSetsHelper
	{
		private readonly DbContext _db;
		private readonly IDataSets _datasets;

		public DataSetsHelper(DbContext db, IDataSets datasets)
		{
			_db = db;
			_datasets = datasets;
		}

		public List<ListItem> GetDataSet<T>(bool refresh = false) where T : class, IObjectBase
		{
			List<ListItem> dataset = null;
			if (refresh || !_datasets.Collection.ContainsKey(typeof(T)))
			{
				IRepository<T> repo = new BaseRepository<T>(_db);
				dataset = repo.ItemList();
				if (refresh)
				{
					_datasets.Collection[typeof(T)] = dataset;
				}
				else
				{
					_datasets.Collection.Add(typeof(T), dataset);
				}

			}
			return dataset;
		}

		public List<ListItem> GetDataSetFromEnum<T>(bool refresh = false)
		{
			List<ListItem> dataset = null;
			if (refresh || !_datasets.Collection.ContainsKey(typeof(T)))
			{
				var values = Enum.GetValues(typeof (T)).Cast<T>();
				dataset = new List<ListItem>(values.Select(e =>
				{
					var enumValue = e as Enum;
					var id = Convert.ToInt32(e);
					var name = enumValue.GetLocalisedLanguageName();
					return new ListItem(id, name);
				}));
				_datasets.Collection[typeof(T)] = dataset;
			}
			return dataset;
		}

		public SelectList GetSelectList<T>(int? selectedId, bool refresh = false) where T : class, IObjectBase
		{
			return new SelectList(GetDataSet<T>(refresh), "Id", "Name", selectedId);
		}

		public SelectList GetSelectListFromEnum<T>(int selectedId, bool refresh = false)
		{
			return new SelectList(GetDataSetFromEnum<T>(refresh), "Id", "Name", selectedId);
		}

		public string GetName<T>(int? selectedId, bool refresh = false) where T : class, IObjectBase
		{
			if (!selectedId.HasValue)
			{
				return string.Empty;
			}

			var item = GetDataSet<T>(refresh).FirstOrDefault(x => x.Id == selectedId.Value);
			if (item != null)
				return item.Name;
			return string.Empty;
		}
	}

}