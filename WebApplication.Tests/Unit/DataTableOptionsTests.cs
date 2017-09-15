using System;
using System.Collections.Generic;
using System.Linq;
using K9.Base.DataAccessLayer.Config;
using K9.Base.DataAccessLayer.Models;
using K9.Base.WebApplication.Exceptions;
using K9.Base.WebApplication.Options;
using Xunit;

namespace K9.WebApplication.Tests.Unit
{
    public class DataTableOptionsTests
	{

		[Fact]
		public void GetColumns_ShouldReturnAllColumns()
		{
			var options = new DataTableOptions<Country>
			{
				ColumnsConfig = new ColumnsConfig()
			};

			var propertyInfos = options.GetColumns();

			Assert.Equal(6, propertyInfos.Count);
			Assert.Equal("TwoLetterCountryCode", propertyInfos.First().Name);
		}

		[Fact]
		public void GetColumns_ShouldOrderColumns_WhenVisibleColumnsIsSet_AndOrderByThem()
		{
			var options = new DataTableOptions<Country>
			{
				ColumnsConfig = new ColumnsConfig(),
				VisibleColumns = new List<string>
				{
					"ThreeLetterCountryCode",
					"Id",
					"TwoLetterCountryCode"
				}
			};

			var propertyInfos = options.GetColumns();

			Assert.Equal(6, propertyInfos.Count);
			Assert.Equal("ThreeLetterCountryCode", propertyInfos.First().Name);
			Assert.Equal("Id", propertyInfos[1].Name);
			Assert.Equal("TwoLetterCountryCode", propertyInfos[2].Name);
		}

		[Fact]
		public void GetColumns_ShouldThrowError_ForInvalidColumn()
		{
			var options = new DataTableOptions<Country>
			{
				ColumnsConfig = new ColumnsConfig(),
				VisibleColumns = new List<string>
				{
					"ThreeLetterCountryCode",
					"InvalidColumn",
					"TwoLetterCountryCode"
				}
			};

			try
			{
				var propertyInfos = options.GetColumns();
			}
			catch (Exception ex)
			{
				Assert.True(ex.GetType() == typeof(InvalidColumnNameException));
			}
		}

	}

}
