
namespace K9.Base.WebApplication.Models
{
	public class MultiSelectItem
	{
		public int Id { get; set; }
		public int ChildId { get; set; }
		public bool IsSelected { get; set; }
		public string Description { get; set; }
	}
}