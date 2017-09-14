
using K9.WebApplication.Enums;

namespace K9.WebApplication.Options
{
	public class AlertOptions
	{
		public string Message { get; set; }
		public string OtherMessage { get; set; }
		public EAlertType AlertType { get; set; }

		public string GetAlertImage()
		{
			switch (AlertType)
			{
				case EAlertType.Unspecified:
					return string.Empty;

				default:
					return $"{AlertType.ToString().ToLower()}.png";
			}
		}
	}
}