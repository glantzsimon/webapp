

using K9.Base.WebApplication.Constants;

namespace K9.Base.WebApplication.Enums
{

	public enum EButtonType
	{
		Submit,
		Create,
		Delete,
		Edit,
		Button
	}

	public enum EButtonClass
	{
		Default,
		Primary,
		Info,
		Warning,
		Success,
		Danger,
		Link,
		Large,
		Small,
		ExtraSmall,
		Block,
		Active,
		Disabled,
		Navbar,
		IconRight
	}

	public enum EInputSize
	{
		Medium,
		Large,
		Small
	}

	public enum EInputWidth
	{
		Default,
		Narrow,
		Medium,
		Wide
	}

	public static class ExtensionMethods
	{
		public static string ToCssClass(this EInputSize size)
		{
			switch (size)
			{
				case EInputSize.Small:
					return "input-sm";

				case EInputSize.Large:
					return "input-lg";

				default:
					return string.Empty;
			}
		}

		public static string ToCssClass(this EInputWidth width)
		{
			switch (width)
			{
				case EInputWidth.Narrow:
					return "col-xs-2";

				case EInputWidth.Medium:
					return "col-xs-3";

				case EInputWidth.Wide:
					return "col-xs-4";

				default:
					return string.Empty;
			}
		}

		public static string ToCssClass(this EButtonClass buttonClass)
		{
			switch (buttonClass)
			{
				case EButtonClass.Default:
					return Bootstrap.Classes.ButtonDefault;

				case EButtonClass.Primary:
					return Bootstrap.Classes.ButtonPrimary;

				case EButtonClass.Info:
					return Bootstrap.Classes.ButtonInfo;

				case EButtonClass.Warning:
					return Bootstrap.Classes.ButtonWarning;

				case EButtonClass.Success:
					return Bootstrap.Classes.ButtonSuccess;

				case EButtonClass.Danger:
					return Bootstrap.Classes.ButtonDanger;

				case EButtonClass.Link:
					return Bootstrap.Classes.ButtonLink;

				case EButtonClass.Large:
					return Bootstrap.Classes.ButtonLarge;

				case EButtonClass.Small:
					return Bootstrap.Classes.ButtonSmall;

				case EButtonClass.ExtraSmall:
					return Bootstrap.Classes.ButtonExtraSmall;

				case EButtonClass.Block:
					return Bootstrap.Classes.ButtonBlock;

				case EButtonClass.Active:
					return Bootstrap.Classes.Active;

				case EButtonClass.Disabled:
					return Bootstrap.Classes.Disabled;

				case EButtonClass.Navbar:
					return Bootstrap.Classes.ButtonNavbar;

				default:
					return string.Empty;
			}
		}

	}
}
