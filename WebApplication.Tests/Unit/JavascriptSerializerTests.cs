using System.Collections.Generic;
using System.Web.Script.Serialization;
using Xunit;

namespace K9.WebApplication.Tests.Unit
{
	public class JavascriptSerializerTests
	{

		[Fact]
		public void OutputOfJavascriptSerializer_ShouldBeArrayOfString()
		{
			var strings = new List<string>
			{
				"one",
				"two",
				"three"
			};

			var jsArray = new JavaScriptSerializer().Serialize(strings.ToArray());

			Assert.Equal("[\"one\",\"two\",\"three\"]", jsArray);
		}

	}

}
