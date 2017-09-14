
using System;

namespace K9.Base.WebApplication.Exceptions
{
	public class InvalidColumnNameException : ApplicationException
	{
		public InvalidColumnNameException(string columnNames) : base($"Invalid column name(s) '{columnNames}'")
		{
		}
	}
}