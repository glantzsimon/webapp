using System.Collections.Generic;

namespace K9.WebApplication.Models
{
	public class ServiceResult
	{
		public ServiceResult()
		{
			Errors = new List<ServiceError>();
		}

		public bool IsSuccess { get; set; }
		public List<ServiceError> Errors { get; set; }
		public object Data { get; set; }
	}
}