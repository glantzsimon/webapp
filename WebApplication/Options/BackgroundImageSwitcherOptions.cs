using System.ComponentModel;
using K9.Base.WebApplication.Enums;

namespace K9.Base.WebApplication.Options
{

	public class BackgroundImageSwitcherOptions : ImageSwitcherOptions
	{

		[DefaultValue(BackgroundImageSwitcherEffect.Fade)]
		public BackgroundImageSwitcherEffect Effect { get; set; }

		[DefaultValue(true)]
		public bool Loop { get; set; }

		public string ElementId { get; set; }

		public BackgroundImageSwitcherOptions(string pathToImages) : base(pathToImages)
		{
			TransitionDuration = 3000;
			Interval = 10000;
			AutoPlay = true;
			Loop = true;
			TransitionOrder = ImageSwitcherTransitionOrder.Random;
		}
	}
}