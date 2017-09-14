using K9.Base.WebApplication.Enums;
using K9.Base.WebApplication.Helpers;
using K9.DataAccessLayer.Models;
using K9.SharedLibrary.Helpers;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Helpers
{
	public class BootstrapButtonTests
	{

		[Fact]
		public void Buttons_ShouldRenderCorrectly()
		{
			var html = HtmlHelper.CreateHtmlHelper(new User());
			var button1 = html.BootstrapActionLinkButton("Test", "Index", "Home", null, "fa-chevron-right", EButtonClass.Large).ToString();
			var button2 = html.BootstrapActionLinkButton("Test", "Index", "Home", null, "fa-chevron-right", EButtonClass.Small, EButtonClass.IconRight).ToString();
			var button3 = html.BootstrapActionLinkButton("Test", "Index", "Home", null, "fa-chevron-right", EButtonClass.Info).ToString();

			Assert.Equal("<a class=\"btn btn-primary btn-lg\" href=\"\"><i class='fa fa-chevron-right'></i> Test</a>", button1);
			Assert.Equal("<a class=\"btn btn-primary btn-sm\" href=\"\">Test <i class='fa fa-chevron-right'></i></a>", button2);
			Assert.Equal("<a class=\"btn btn-info\" href=\"\"><i class='fa fa-chevron-right'></i> Test</a>", button3);
		}
		
	}
}
