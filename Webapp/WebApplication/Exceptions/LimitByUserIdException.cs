
using System;

namespace K9.WebApplication.Exceptions
{
	public class LimitByUserIdException : ApplicationException
	{
		public LimitByUserIdException()
			: base("Cannot limit by UerId for an entity which does not have a UserId property.")
		{
		}
	}
}