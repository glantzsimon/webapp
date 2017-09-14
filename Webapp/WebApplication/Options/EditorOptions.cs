using K9.WebApplication.Enums;

namespace K9.WebApplication.Options
{
	public class EditorOptions
	{
		public EInputSize InputSize { get; set; }
		public EInputWidth InputWidth { get; set; }
		public string PlaceHolder { get; set; }
		public string Label { get; set; }
		public bool IsReadOnly { get; set; }
	}
}