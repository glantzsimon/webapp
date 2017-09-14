using System.Globalization;
using K9.SharedLibrary.Extensions;
using Xunit;

namespace K9.WebApplication.Tests.Unit
{
	public class GlobalisationTests
	{

		[Fact]
		public void GetLocaleLanguage_ReturnsCorrectLanguageName()
		{
			var cultureInfo = new CultureInfo("fr-fr");

			Assert.Equal("French", cultureInfo.GetLocaleLanguage());
		}

		[Fact]
		public void LanguageCode_ShouldBeTwoLetters()
		{
		    var cultureInfo = new CultureInfo("en-gb");

            Assert.Equal("en", cultureInfo.TwoLetterISOLanguageName);
		}
	}

}
