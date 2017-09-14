
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using K9.SharedLibrary.Attributes;
using K9.SharedLibrary.Extensions;
using K9.SharedLibrary.Models;
using K9.WebApplication.Exceptions;
using NLog;

namespace K9.WebApplication.Helpers
{
	public class DataTableAjaxHelper<T> : IDataTableAjaxHelper<T> where T : class
	{
		private const int DefaultTotalRows = 500;
		private const string DrawKey = "draw";
		private const string StartKey = "start";
		private const string LengthKey = "length";
		private const string SearchKey = "search[value]";
		private const string SearchRegexKey = "search[regex]";
		private const string SortedColumnIndexKey = "order[0][column]";
		private const string SortedColumnDirectionKey = "order[0][dir]";

		private readonly ILogger _logger;
		private readonly IColumnsConfig _columnsConfig;
		private readonly List<IDataTableColumnInfo> _columnInfos = new List<IDataTableColumnInfo>();
		private NameValueCollection _queryString;
		private int _draw;
		private int _start;
		private int _length;
		private string _searchValue;
		private bool _isRegexSearch;
		private int _orderByColumnIndex;
		private string _orderByDirection;

		private delegate void DataColumnInfoUpdateDelegate(IDataTableColumnInfo columnInfo, object value);

		public DataTableAjaxHelper(ILogger logger, IColumnsConfig columnsConfig)
		{
			_logger = logger;
			_columnsConfig = columnsConfig;
		}

		public void LoadQueryString(NameValueCollection queryString)
		{
			_queryString = queryString;

			_draw = GetIntegerValueFromQueryString(DrawKey);
			_start = GetIntegerValueFromQueryString(StartKey);
			_length = GetIntegerValueFromQueryString(LengthKey);
			_searchValue = GetValueFromQueryString(SearchKey);
			_isRegexSearch = GetBooleanValueFromQueryString(SearchRegexKey);
			_orderByColumnIndex = GetIntegerValueFromQueryString(SortedColumnIndexKey);
			_orderByDirection = GetValueFromQueryString(SortedColumnDirectionKey).ToUpper();
			SetColumnInfosFromQueryString();
		}

		public string GetQuery(bool selectAllColumns = false, int? limitByUserId = null)
		{
			return "WITH RESULTS AS " + $"(SELECT {GetSelectColumns(selectAllColumns)}, ROW_NUMBER() OVER " +
			       $"(ORDER BY {OrderByColumnName} {OrderByDirection}) AS RowNum " + $"FROM {GetFrom()} " +
			       $"{GetWhereClause(false, limitByUserId)}) " + "SELECT * FROM RESULTS " +
			       $"WHERE RowNum BETWEEN {Start} AND {PageEnd}";
		}

		public int Draw => _draw;

	    public int Start => _start;

	    public int Length => _length == 0 ? DefaultTotalRows : _length;

	    public int PageEnd => _start + _length;

	    public string SearchValue => _searchValue;

	    public IStatelessFilter StatelessFilter { get; set; }

		public bool IsRegexSearch => _isRegexSearch;

	    public List<IDataTableColumnInfo> ColumnInfos => _columnInfos;

	    public int OrderByColumnIndex => _orderByColumnIndex;

	    public string OrderByColumnName
		{
			get
			{
				try
				{
					return GetFullyQualifiedOrderByColumnName();
				}
				catch (Exception ex)
				{
					_logger.Error(ex.GetFullErrorMessage());
					throw new Exception("Invalid order by column index");
				}
			}
		}

		public string OrderByDirection => _orderByDirection;

	    public string GetWhereClause(bool ignoreChildTables = false, int? limitByUserId = null)
		{
			var sb = new StringBuilder();
			var parentType = typeof(T);
			var linkedTableInfos = GetLinkedTableInfos();

			foreach (var columnInfo in GetDataBoundColumnInfosNotIdColumns())
			{
				var linkedTable = linkedTableInfos.FirstOrDefault(c => c.ColumnName == columnInfo.Data);
				var searchValue = !string.IsNullOrEmpty(SearchValue) ? GetLikeSearchValue() : (!string.IsNullOrEmpty(columnInfo.SearchValue) ? columnInfo.GetLikeSearchValue() : string.Empty);

				if (!string.IsNullOrEmpty(searchValue))
				{
					if (linkedTable != null)
					{
						if (!ignoreChildTables)
						{
							sb.Append(sb.Length == 0 ? "WHERE (" : " OR ");
							sb.AppendFormat("[{0}].[{1}] LIKE '{2}'", linkedTable.LinkedTableName, linkedTable.LinkedTableColumnName, searchValue);
						}
					}
					else
					{
						sb.Append(sb.Length == 0 ? "WHERE (" : " OR ");
						sb.AppendFormat("[{0}].[{1}] LIKE '{2}'", parentType.Name, columnInfo.Data, searchValue);
					}
				}
			}

			if (sb.Length > 0)
			{
				sb.Append(")");
			}

			if (limitByUserId.HasValue)
			{
				if (!typeof(T).ImplementsIUserData())
				{
					throw new LimitByUserIdException();
				}
				sb.Append(sb.Length == 0 ? "WHERE " : " AND ");
				sb.AppendFormat("[{0}].[UserId] = {1}", parentType.Name, limitByUserId.Value);
			}

			if (StatelessFilter != null && StatelessFilter.IsSet())
			{
				sb.Append(sb.Length == 0 ? "WHERE " : " AND ");
				sb.AppendFormat("[{0}].[{1}] = {2}", parentType.Name, StatelessFilter.Key, StatelessFilter.Id);
			}

			return sb.ToString();
		}

		private static PropertyInfo[] _propertyInfos;
		private static PropertyInfo[] GetModelProperties()
		{
			_propertyInfos = _propertyInfos ?? typeof(T).GetProperties();
			return _propertyInfos;
		}

		private string GetFullyQualifiedOrderByColumnName()
		{
			if (ColumnInfos.Any())
			{
				var columnInfo = GetDataBoundColumnInfosNotIgnored()[_orderByColumnIndex];
				var linkedTableInfos = GetLinkedTableInfos();
				var parentType = typeof(T);
				var columnName = columnInfo.Data;
				var linkedTable = linkedTableInfos.FirstOrDefault(x => x.ColumnName == columnName);

				if (linkedTable != null)
				{
					return $"[{linkedTable.LinkedTableAlias}].[{linkedTable.LinkedTableColumnName}]";
				}

				return $"[{parentType.Name}].[{columnName}]";
			}

			return string.Empty;
		}

		private string GetSelectColumns(bool selectAllColumns = false)
		{
			var sb = new StringBuilder();
			var parentType = typeof(T);

			if (selectAllColumns)
			{
				sb.AppendFormat("[{0}].*", parentType.Name);
			}
			else
			{
				foreach (var columnInfo in GetDataBoundColumnInfosNotIgnored())
				{
					sb.Append(sb.Length == 0 ? "" : ", ");
					sb.AppendFormat("[{0}].[{1}]", parentType.Name, columnInfo.Data);
				}
			}

			foreach (var item in GetLinkedTableInfos())
			{
				sb.Append(", ");
				sb.AppendFormat("[{0}].[Name] AS [{1}]", item.LinkedTableAlias, item.ColumnName);
			}

			return sb.ToString();
		}

		private string GetFrom()
		{
			var foreignKeyColumns = GetLinkedTableInfos();
			return foreignKeyColumns.Any()
				? GetFromWithJoins()
				: $"[{typeof(T).Name}]";
		}

		private string GetFromWithJoins()
		{
			var sb = new StringBuilder();
			var parentType = typeof(T);
			var parentName = parentType.Name;
			sb.AppendFormat("[{0}]", parentName);

			foreach (var item in GetLinkedTableInfos())
			{
				sb.AppendFormat(" JOIN [{0}] AS [{1}] ON [{1}].[Id] = [{2}].[{3}]", item.LinkedTableName, item.LinkedTableAlias, parentName, item.ForeignKey);
			}

			return sb.ToString();
		}

		private List<LinkedTableInfo> _linkedTableInfos;
		private List<LinkedTableInfo> GetLinkedTableInfos()
		{
			if (_linkedTableInfos == null)
			{
				var linkedColumnAttributes = typeof(T).GetPropertiesAndAttributesWithAttribute<LinkedColumnAttribute>();
				_linkedTableInfos = new List<LinkedTableInfo>();

				foreach (var value in linkedColumnAttributes)
				{
					var linkedColumnAttribute = value.Key;
					var tableName = linkedColumnAttribute.LinkedTableName;
					var alias = tableName;
					var i = 1;

					while (_linkedTableInfos.Exists(t => t.LinkedTableAlias == alias))
					{
						alias = $"{tableName}{i}";
						i++;
					}

					_linkedTableInfos.Add(new LinkedTableInfo
					{
						LinkedTableName = tableName,
						LinkedTableAlias = alias,
						LinkedTableColumnName = linkedColumnAttribute.LinkedColumnName,
						ColumnName = value.Value.Name,
						ForeignKey = string.IsNullOrEmpty(linkedColumnAttribute.ForeignKey) ? $"{linkedColumnAttribute.LinkedTableName}Id"
						    : linkedColumnAttribute.ForeignKey
					});
				}
			}
			return _linkedTableInfos;
		}

		private List<IDataTableColumnInfo> GetDataBoundColumnInfos()
		{
			return ColumnInfos.Where(c => GetModelProperties()
				.Where(p => p.IsDataBound()).Select(p => p.Name).Contains(c.Data)).ToList();
		}

		private List<IDataTableColumnInfo> GetDataBoundColumnInfosNotIgnored()
		{
			return GetDataBoundColumnInfos().Where(c => !_columnsConfig.ColumnsToIgnore.Contains(c.Data)).ToList();
		}

		private List<IDataTableColumnInfo> GetDataBoundColumnInfosNotIdColumns()
		{
			return GetDataBoundColumnInfosNotIgnored().Where(c => GetModelProperties()
				.Where(p => !p.IsPrimaryKey() && !p.IsForeignKey()).Select(p => p.Name).Contains(c.Data)).ToList();
		}

		/// <summary>
		/// Takes into account whether it is a regex search or not
		/// </summary>
		/// <returns></returns>
		private string GetLikeSearchValue()
		{
			return IsRegexSearch ? $"%[{SearchValue}]%" : $"%{SearchValue}%";
		}

		private string GetValueFromQueryString(string key)
		{
			return _queryString[key] ?? string.Empty;
		}

		private int GetIntegerValueFromQueryString(string key)
		{
			int value;
			int.TryParse(GetValueFromQueryString(key), out value);
			return value;
		}

		private bool GetBooleanValueFromQueryString(string key)
		{
			bool value;
			bool.TryParse(GetValueFromQueryString(key), out value);
			return value;
		}

		private void SetColumnInfosFromQueryString()
		{
			var columnDataRegex = new Regex(@"columns\[[0-9]\]\[data\]");
			var columnNameRegex = new Regex(@"columns\[[0-9]\]\[name\]");
			var columnSearchValueRegex = new Regex(@"columns\[[0-9]\]\[search\]\[value\]");
			var columnIsRegexSearchRegex = new Regex(@"columns\[[0-9]\]\[search\]\[regex\]");

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnDataRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateData(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnNameRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateName(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnSearchValueRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateSearchValue(data.ToString());
			});

			AddColumnInfoData(_queryString.AllKeys.Where(k => columnIsRegexSearchRegex.IsMatch(k)).ToList(), (columnInfo, data) =>
			{
				columnInfo.UpdateIsSearchRegex(bool.Parse(data.ToString()));
			});
		}

		private void AddColumnInfoData(List<string> keys, DataColumnInfoUpdateDelegate columnUpdater)
		{
			foreach (var key in keys)
			{
				var columnData = _queryString[key];
				var columnIndex = GetColumnIndexFromKey(key);
				var columnInfo = GetColumnInfoAtIndex(columnIndex);
				columnUpdater(columnInfo, columnData);
			}
		}

		private IDataTableColumnInfo GetColumnInfoAtIndex(int index)
		{
			var columnInfo = ColumnInfos.FirstOrDefault(c => c.Index == index);
			if (columnInfo == null)
			{
				columnInfo = new DataTableColumnInfo(index);
				ColumnInfos.Add(columnInfo);
			}
			return columnInfo;
		}

		private int GetColumnIndexFromKey(string key)
		{
			var columnIndexRegex = new Regex(@"\[([0-9])\]");
			int index = 0;
			var columnIndexValue = columnIndexRegex.Match(key).Groups[1].Value;
			int.TryParse(columnIndexValue, out index);
			return index;
		}
	}

	public class DataTableColumnInfo : IDataTableColumnInfo
	{
		private readonly int _index;
		private string _data;
		private string _name;
		private string _searchValue;
		private bool _isSearchRegex;

		public DataTableColumnInfo(int index)
		{
			_index = index;
		}

		public int Index => _index;

	    public string Data => _data;

	    public string Name => _name;

	    public string SearchValue => _searchValue;

	    public bool IsRegexSearch => _isSearchRegex;

	    public bool IsDatabound { get; set; }

		public string Renderer { get; set; }

		public bool IsVisible { get; set; }

		public string DataType { get; set; }

		public void UpdateData(string data)
		{
			_data = data;
		}

		public void UpdateName(string name)
		{
			_name = name;
		}

		public void UpdateSearchValue(string searchValue)
		{
			_searchValue = searchValue;
		}

		public void UpdateIsSearchRegex(bool value)
		{
			_isSearchRegex = value;
		}

		/// <summary>
		/// Takes into account whether it is a regex search or not
		/// </summary>
		/// <returns></returns>
		public string GetLikeSearchValue()
		{
			return IsRegexSearch ? $"%[{SearchValue}]%" : $"%{SearchValue}%";
		}
	}
}