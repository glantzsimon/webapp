using K9.SharedLibrary.Helpers;
using K9.SharedLibrary.Models;
using K9.WebApplication.Helpers;
using NLog;

namespace K9.WebApplication.UnitsOfWork
{
	public class ControllerPackage<T> : IControllerPackage<T>
		where T : class, IObjectBase
	{
		public IRepository<T> Repository { get; set; }
		public ILogger Logger { get; set; }
		public IDataTableAjaxHelper<T> AjaxHelper { get; set; }
		public IDataSetsHelper DataSetsHelper { get; set; }
		public IRoles Roles { get; set; }
		public IFileSourceHelper FileSourceHelper { get; set; }

		public ControllerPackage(IRepository<T> repository, ILogger logger, IDataTableAjaxHelper<T> ajaxHelper, IDataSetsHelper dataSetsHelper, IRoles roles, IFileSourceHelper fileSourceHelper)
		{
			Repository = repository;
			Logger = logger;
			AjaxHelper = ajaxHelper;
			DataSetsHelper = dataSetsHelper;
			Roles = roles;
			FileSourceHelper = fileSourceHelper;
		}
	}
}