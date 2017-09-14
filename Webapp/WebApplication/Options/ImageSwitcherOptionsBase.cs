using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Enums;

namespace K9.WebApplication.Options
{

	public abstract class ImageSwitcherOptionsBase
	{
		private readonly string _pathToImages;
		private List<AssetInfo> _images;

		protected ImageSwitcherOptionsBase(string pathToImages)
		{
			_pathToImages = pathToImages;

			LoadImages();

			AutoPlay = true;
			TransitionOrder = ImageSwitcherTransitionOrder.Sequence;
			TransitionDuration = 500;
			Interval = 8000;
		}

		public string PathToImages => _pathToImages;

	    public List<AssetInfo> Images => _images;

	    [DefaultValue(ImageSwitcherTransitionOrder.Sequence)]
		public ImageSwitcherTransitionOrder TransitionOrder { get; set; }

		[DefaultValue(true)]
		public bool AutoPlay { get; set; }

		[DefaultValue(500)]
		public int TransitionDuration { get; set; }

		[DefaultValue(8000)]
		public int Interval { get; set; }

		private void LoadImages()
		{
			_images = ContentHelper.GetImageFiles(PathToImages);
		}

		public List<string> GetImageSourceList()
		{
			return Images.Select(i => i.Src).ToList();
		}

		public MvcHtmlString GetImagesArray()
		{
			return MvcHtmlString.Create(new JavaScriptSerializer().Serialize(Images.Select(i => i.Src).ToArray()));
		}
	}
}