
using System.Collections.Generic;
using System.Collections.Specialized;
using K9.SharedLibrary.Models;

namespace K9.WebApplication.Helpers
{
	public interface IDataTableAjaxHelper<T> where T : class 
	{
		int Draw { get; }
		int Start { get; }
		int Length { get; }
		string SearchValue { get; }
		IStatelessFilter StatelessFilter { get; set; }
		List<IDataTableColumnInfo> ColumnInfos { get; }
		void LoadQueryString(NameValueCollection queryString);
		string GetQuery(bool selectAllColumns = false, int? limitByUserId = null);
		string GetWhereClause(bool ignoreChildTables = false, int? limitByUser = null);
	}

	public interface IDataTableColumnInfo
	{
		int Index { get; }
		string Data { get; }
		string Name { get; }
		string SearchValue { get; }
		bool IsRegexSearch { get; }
		bool IsDatabound { get; set; }
		string Renderer { get; set; }
		string DataType { get; set; }
		bool IsVisible { get; set; }

		void UpdateData(string data);
		void UpdateName(string name);
		void UpdateSearchValue(string searchValue);
		void UpdateIsSearchRegex(bool value);
		string GetLikeSearchValue();
	}
}