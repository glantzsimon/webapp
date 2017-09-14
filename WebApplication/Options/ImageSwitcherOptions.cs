using System.ComponentModel;
using System.Linq;
using K9.Base.WebApplication.Enums;

namespace K9.Base.WebApplication.Options
{

	public class ImageSwitcherOptions : ImageSwitcherOptionsBase
	{

		[DefaultValue(false)]
		public bool ShowPager { get; set; }

		[DefaultValue(false)]
		public bool ShowNav { get; set; }

		[DefaultValue(false)]
		public bool ShowPauseButton { get; set; }

		[DefaultValue(false)]
		public bool PauseOnHover { get; set; }

		[DefaultValue("Previous")]
		public string PreviousText { get; set; }

		[DefaultValue("Next")]
		public string NextText { get; set; }

		[DefaultValue(ImageSwitcherSizeMode.Auto)]
		public ImageSwitcherSizeMode SizeMode { get; set; }

		[DefaultValue(true)]
		public bool RoundCorners { get; set; }


		public ImageSwitcherOptions(string pathToImages)
			: base(pathToImages)
		{
			AutoPlay = true;
			TransitionOrder = ImageSwitcherTransitionOrder.Sequence;
			ShowNav = false;
			PauseOnHover = true;
			PreviousText = "Previous";
			NextText = "Next";
			SizeMode = ImageSwitcherSizeMode.FullScreen;
			RoundCorners = true;
			PauseOnHover = false;
		}

		public string RoundCornersStyle => RoundCorners ? "border-radius: 8px;" : "";

	    public string MaxWidthStyle
		{
			get
			{
				var firstImage = Images.FirstOrDefault();
				return SizeMode == ImageSwitcherSizeMode.Auto && firstImage != null
					? $"{firstImage.ImageInfo.Width}px"
				    : "";
			}
		}

		public string Random => (TransitionOrder == ImageSwitcherTransitionOrder.Random).ToString().ToLower();
	}
}