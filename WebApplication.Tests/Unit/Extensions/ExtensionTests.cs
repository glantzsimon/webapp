using K9.Base.WebApplication.Controllers;
using K9.Base.WebApplication.Extensions;
using Xunit;

namespace K9.WebApplication.Tests.Unit.Extensions
{
	public class ExtensionTests
	{
		
		[Fact]
		public void GetControllerName_ShouldReturnNameOfController_ForUseInRouting()
		{
			Assert.Equal("Base", typeof(BaseController).GetControllerName());
		}
		
	}

}
