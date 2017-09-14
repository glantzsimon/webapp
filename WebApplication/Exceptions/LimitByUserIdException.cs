
using System;

namespace K9.Base.WebApplication.Exceptions
{
	public class LimitByUserIdException : ApplicationException
	{
		public LimitByUserIdException()
			: base("Cannot limit by UerId for an entity which does not have a UserId property.")
		{
		}
	}
}