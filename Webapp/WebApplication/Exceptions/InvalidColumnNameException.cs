
using System;

namespace K9.WebApplication.Exceptions
{
	public class InvalidColumnNameException : ApplicationException
	{
		public InvalidColumnNameException(string columnNames) : base($"Invalid column name(s) '{columnNames}'")
		{
		}
	}
}