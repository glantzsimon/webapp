using K9.Base.WebApplication.Constants;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Models
{
	public class Button : IButton
	{
		public string Text { get; set; }
		public string Action { get; set; }
		public string IconCssClass { get; set; }
		public EButtonType ButtonType { get; set; }

		public string ButtonCssClass => GetButtonCssClass();

	    private string GetButtonCssClass()
		{
			switch (ButtonType)
			{
				case EButtonType.Primary:
					return Bootstrap.Classes.ButtonPrimary;

				case EButtonType.Success:
					return Bootstrap.Classes.ButtonSuccess;

				case EButtonType.Info:
					return Bootstrap.Classes.ButtonInfo;

				case EButtonType.Warning:
					return Bootstrap.Classes.ButtonWarning;

				case EButtonType.Danger:
					return Bootstrap.Classes.ButtonDanger;

				case EButtonType.Link:
					return Bootstrap.Classes.ButtonLink;

				default:
					return Bootstrap.Classes.ButtonDefault;
			}
		}
	}
}