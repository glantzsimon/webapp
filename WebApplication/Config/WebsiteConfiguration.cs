
namespace K9.Base.WebApplication.Config
{
	public class WebsiteConfiguration
	{
		public string CompanyLogoUrl { get; set; }
		public string CompanyName { get; set; }
	    public string CompanyShortDescription { get; set; }
        public string SupportEmailAddress { get; set; }

		public static WebsiteConfiguration Instance { get; set; }
	}

}