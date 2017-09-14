
namespace K9.WebApplication.Models
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