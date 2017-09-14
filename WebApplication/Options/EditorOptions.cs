using K9.Base.WebApplication.Enums;

namespace K9.Base.WebApplication.Options
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