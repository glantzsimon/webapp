using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using K9.Base.WebApplication.Enums;
using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;

namespace K9.Base.WebApplication.Options
{

	public class CarouselOptions
	{

		private readonly string _pathToImages;
		private readonly int _imageWidth;
		private readonly string _pathToFullSizeImages;
		private List<AssetInfo> _images;
		private List<AssetInfo> _fullSizeImages;
		private Guid _id = Guid.NewGuid();

		public CarouselOptions(string pathToImages, string fullSizeImageFolderName = "full-size", int imageWidth = 70, EImageSizing sizing = EImageSizing.Horizontal)
		{
			_pathToImages = pathToImages;
			_imageWidth = imageWidth;
			ImageSizing = sizing;
			_pathToFullSizeImages = Path.Combine(_pathToImages, fullSizeImageFolderName);
			LoadImages();
		}

		public string Id => _id.ToString();

	    public string PathToImages => _pathToImages;

	    public List<AssetInfo> Images => _images;

	    public List<AssetInfo> FullSizeImages => _fullSizeImages;

	    public EImageSizing ImageSizing { get; set; }
		
		public int ImageWidth => _imageWidth;

	    public AssetInfo GetFullSizeImage(string imageName)
		{
			var fullSizeImage = _fullSizeImages.FirstOrDefault(i => i.FileName == imageName);
			return fullSizeImage ?? _images.FirstOrDefault(i => i.FileName == imageName);
		}

		private void LoadImages()
		{
			_images = ContentHelper.GetImageFiles(_pathToImages);
			_fullSizeImages = Directory.Exists(_pathToFullSizeImages) ? ContentHelper.GetImageFiles(_pathToFullSizeImages) : new List<AssetInfo>();
		}

	}
}