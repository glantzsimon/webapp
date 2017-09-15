using System.ComponentModel.DataAnnotations;
using K9.Base.Globalisation;

namespace K9.Base.WebApplication.ViewModels
{
	public class ContactUsViewModel
	{
		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.NameLabel)]
		public string Name { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.EmailAddressLabel)]
		public string EmailAddress { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.SubjectLabel)]
		public string Subject { get; set; }

		[Required(ErrorMessageResourceType = typeof(Dictionary), ErrorMessageResourceName = Strings.ErrorMessages.FieldIsRequired)]
		[Display(ResourceType = typeof(Dictionary), Name = Strings.Labels.BodyLabel)]
		[DataType(DataType.MultilineText)]
		public string Body { get; set; }
		
	}
}