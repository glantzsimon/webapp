
namespace K9.Base.WebApplication.Models
{
	public class Crumb
	{
		public bool IsHome { get; set; }
		public string Label { get; set; }
		public string ActionName { get; set; }
		public string ControllerName { get; set; }
		public bool IsActive { get; set; }
	}
}