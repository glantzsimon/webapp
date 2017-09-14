using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.UnitsOfWork
{
	public interface IControllerPackage<T>
		where T : class, IObjectBase
	{
		IRepository<T> Repository { get; set; }
		ILogger Logger { get; set; }
		IDataTableAjaxHelper<T> AjaxHelper { get; set; }
		IDataSetsHelper DataSetsHelper { get; set; }
		IRoles Roles { get; set; }
		IFileSourceHelper FileSourceHelper { get; set; }
	}
}