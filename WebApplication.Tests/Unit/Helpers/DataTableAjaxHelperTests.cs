using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using K9.Base.WebApplication.Exceptions;
using K9.Base.WebApplication.Helpers;
using K9.DataAccessLayer.Config;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Extensions;
using Moq;
using NLog;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
	public class DataTableAjaxHelperTests
	{

		[Fact]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", "search"},
				{"search[regex]", "true"},
				{"order[0][column]", "1"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", "gb"},
				{"columns[0][search][regex]", "true"},
				{"columns[1][data]", "ThreeLetterCountryCode"},
				{"columns[1][name]", "Three Letter Country Code"},
				{"columns[1][search][value]", "gb"},
				{"columns[1][search][regex]", "false"},
				{"columns[2][data]", "Test"},
				{"columns[2][name]", "Test"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal(1, helper.Draw);
			Assert.Equal(0, helper.Start);
			Assert.Equal(10, helper.Length);
			Assert.Equal("search", helper.SearchValue);
			Assert.True(helper.IsRegexSearch);
			Assert.Equal(1, helper.OrderByColumnIndex);
			Assert.Equal("ASC", helper.OrderByDirection);
			Assert.Equal(3, helper.ColumnInfos.Count);

			var firstColumnInfo = helper.ColumnInfos.First();
			Assert.Equal("TwoLetterCountryCode", firstColumnInfo.Data);
			Assert.Equal("Two Letter Country Code", firstColumnInfo.Name);
			Assert.Equal("gb", firstColumnInfo.SearchValue);
			Assert.True(firstColumnInfo.IsRegexSearch);
			Assert.Equal("WITH RESULTS AS " +
							"(SELECT [Country].[TwoLetterCountryCode], [Country].[ThreeLetterCountryCode], ROW_NUMBER() OVER " +
							"(ORDER BY [Country].[ThreeLetterCountryCode] ASC) AS RowNum " +
							"FROM [Country] " +
							"WHERE ([Country].[TwoLetterCountryCode] LIKE '%[search]%' " +
							"OR [Country].[ThreeLetterCountryCode] LIKE '%[search]%')) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery());
		}

		[Fact]
		public void DataTableAjaxHelper_ShouldReturnTheCorrectSqlQuery()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "40"},
				{"length", "20"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "1"},
				{"order[0][dir]", "desc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", "gb"},
				{"columns[0][search][regex]", "true"},
				{"columns[1][data]", "ThreeLetterCountryCode"},
				{"columns[1][name]", "Three Letter Country Code"},
				{"columns[1][search][value]", "gb"},
				{"columns[1][search][regex]", "false"},
				{"columns[2][data]", "Test"},
				{"columns[2][name]", "Test"},
				{"columns[2][search][value]", ""},
				{"columns[2][search][regex]", "false"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal("WITH RESULTS AS " +
							"(SELECT [Country].[TwoLetterCountryCode], [Country].[ThreeLetterCountryCode], ROW_NUMBER() OVER " +
							"(ORDER BY [Country].[ThreeLetterCountryCode] DESC) AS RowNum " +
							"FROM [Country] " +
							"WHERE ([Country].[TwoLetterCountryCode] LIKE '%[gb]%' " +
							"OR [Country].[ThreeLetterCountryCode] LIKE '%gb%')) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 40 AND 60", helper.GetQuery());
		}

		[Fact]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions_WithNoWhereClause()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", ""},
				{"columns[0][search][regex]", "true"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal("WITH RESULTS AS " +
							"(SELECT [Country].[TwoLetterCountryCode], ROW_NUMBER() OVER " +
							"(ORDER BY [Country].[TwoLetterCountryCode] ASC) AS RowNum " +
							"FROM [Country] ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery());
		}

		[Fact]
		public void ShouldMap_DataTableQueryString_ToDataTableAjaxOptions_AndReturnAllColumns()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "TwoLetterCountryCode"},
				{"columns[0][name]", "Two Letter Country Code"},
				{"columns[0][search][value]", ""},
				{"columns[0][search][regex]", "true"}
			};

			var helper = new DataTableAjaxHelper<Country>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal("WITH RESULTS AS " +
							"(SELECT [Country].*, ROW_NUMBER() OVER " +
							"(ORDER BY [Country].[TwoLetterCountryCode] ASC) AS RowNum " +
							"FROM [Country] ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery(true));
		}
        
		[Fact]
		public void GetAllProperties_IncludingInherited_FromModel()
		{
			var columnsConfig = new ColumnsConfig();
			var columns = typeof(UserRole).GetProperties()
						.Where(p => !p.IsVirtual() && !columnsConfig.ColumnsToIgnore.Contains(p.Name))
						.ToList();

			Assert.True(columns.Select(c => c.Name).Contains("RoleId"));
		}

		[Fact]
		public void GetPropertiesAndAttributesWithAttribute_ShouldReturnAllForeignKeyAttributes_AndProperties()
		{
			var dictionary = typeof(UserRole).GetPropertiesAndAttributesWithAttribute<ForeignKeyAttribute>();

			Assert.Equal(2, dictionary.Count);
		}

		[Fact]
		public void GetQuery_ShouldReturnSQL_IncludingLinkedTables()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "Id"},
				{"columns[0][name]", "Id"}
			};

			var helper = new DataTableAjaxHelper<UserRole>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal("WITH RESULTS AS " +
							"(SELECT [UserRole].*, " +
							"[User].[Name] AS [UserName], " +
							"[Role].[Name] AS [RoleName], " +
							"ROW_NUMBER() OVER " +
                            "(ORDER BY [UserRole].[Id] ASC) AS RowNum " +
                            "FROM [UserRole] " +
                            "JOIN [User] AS [User] ON [User].[Id] = [UserRole].[UserId] " +
							"JOIN [Role] AS [Role] ON [Role].[Id] = [UserRole].[RoleId] ) " +
							"SELECT * FROM RESULTS " +
							"WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery(true));
		}

		[Fact]
		public void EnsureNameProperty_IsIncludedInProperties()
		{
			var props = typeof(Country).GetProperties();

			Assert.Equal(1, props.Count(p => p.Name == "Name"));
		}

		[Fact]
		public void ShouldThrowError_WhenTryingToPassUserId_AndNoUserIdColumnFound()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "Id"},
				{"columns[0][name]", "Id"}
			};

			var helper = new DataTableAjaxHelper<Message>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			try
			{
				helper.GetQuery(true, 4);
			}
			catch (Exception ex)
			{
				Assert.True(ex.GetType() == typeof(InvalidColumnNameException));
			}
		}

		[Fact]
		public void WhereClause_ShouldIncludeUserIdFilter()
		{
			var querystring = new NameValueCollection
			{
				{"draw", "1"},
				{"start", "0"},
				{"length", "10"},
				{"search[value]", ""},
				{"search[regex]", "false"},
				{"order[0][column]", "0"},
				{"order[0][dir]", "asc"},
				{"columns[0][data]", "Id"},
				{"columns[0][name]", "Id"}
			};

			var helper = new DataTableAjaxHelper<Message>(new Mock<ILogger>().Object, new ColumnsConfig());
			helper.LoadQueryString(querystring);

			Assert.Equal("WITH RESULTS AS " +
			                "(SELECT [Message].*, " +
			                "[User].[Name] AS [SentToUserName], " +
			                "[User1].[Name] AS [SentByUserName], " +
			                "[User2].[Name] AS [UserName], " +
			                "ROW_NUMBER() OVER (ORDER BY [Message].[Id] ASC) AS RowNum " +
			                "FROM [Message] " +
			                "JOIN [User] AS [User] ON [User].[Id] = [Message].[SentToUserId] " +
			                "JOIN [User] AS [User1] ON [User1].[Id] = [Message].[SentByUserId] " +
			                "JOIN [User] AS [User2] ON [User2].[Id] = [Message].[UserId] " +
			                "WHERE [Message].[UserId] = 4) " +
			                "SELECT * FROM RESULTS WHERE RowNum BETWEEN 0 AND 10", helper.GetQuery(true, 4));
		}

	}

}
