
using K9.SharedLibrary.Models;

namespace K9.WebApplication.EventArgs
{
	public class CrudEventArgs : System.EventArgs
	{

		public IObjectBase Item { get; set; }

	}
}