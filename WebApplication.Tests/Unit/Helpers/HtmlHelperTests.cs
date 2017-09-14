using K9.DataAccessLayer.Models;
using Xunit;
using HtmlHelper = K9.SharedLibrary.Helpers.HtmlHelper;

namespace K9.WebApplication.Tests.Unit.Helpers
{
	public class HtmlHelperTests
	{

		[Fact]
		public void HtmlHelper_ShouldWriteToStream()
		{
			var html = HtmlHelper.CreateHtmlHelper(new User());

			html.ViewContext.Writer.Write("test");

			Assert.Equal("test", html.GetOutputFromStream());
		}
		
	}
}
