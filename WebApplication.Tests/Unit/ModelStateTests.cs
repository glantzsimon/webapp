using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using K9.Base.WebApplication.Extensions;
using K9.DataAccessLayer.Models;
using Xunit;

namespace K9.WebApplication.Tests.Unit
{
	public class ModelStateTests
	{

		[Fact]
		public void ModelState_AddErrorMessageFromException_ShouldRenderCorrectMessage()
		{
		    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-gb");

            var modelState = new ModelStateDictionary();
			var exception = new Exception("An error occurred while updating the entries. See the inner exception for details. Cannot insert duplicate key row in object 'dbo.Country' with unique index 'IX_TwoLetterCountryCode'. The statement has been terminated.");
			modelState.AddErrorMessageFromException(exception, new Country
			{
					TwoLetterCountryCode = "SDF"
			});

			Assert.Equal("A country with the Two Letter Country Code 'SDF' already exists.", modelState["TwoLetterCountryCode"].Errors.FirstOrDefault()?.ErrorMessage);
		}

		[Fact]
		public void ModelState_AddErrorMessageFromException_ShouldRenderCorrectMessage_InFrench()
		{
			Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
			var modelState = new ModelStateDictionary();
			var exception = new Exception("An error occurred while updating the entries. See the inner exception for details. Cannot insert duplicate key row in object 'dbo.Country' with unique index 'IX_TwoLetterCountryCode'. The statement has been terminated.");
			modelState.AddErrorMessageFromException(exception, new Country
			{
				TwoLetterCountryCode = "SDF"
			});

			Assert.Equal("Un pays avec le Code Pays à Deux Lettres 'SDF' existe déjà.", modelState["TwoLetterCountryCode"].Errors.FirstOrDefault()?.ErrorMessage);
		}

	}

}
