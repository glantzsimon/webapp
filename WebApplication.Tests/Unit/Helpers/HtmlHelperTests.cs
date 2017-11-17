using K9.Base.DataAccessLayer.Models;
using K9.WebApplication.Tests.Shared;
using Xunit;

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
