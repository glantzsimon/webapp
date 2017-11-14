using System;

namespace K9.Base.WebApplication.Models
{
    public class ServiceError
	{
		public string FieldName { get; set; }
		public string ErrorMessage { get; set; }
	    public Exception Exception { get; set; }
	    public Object Data { get; set; }
	}
}