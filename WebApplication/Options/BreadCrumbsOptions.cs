using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using K9.Base.WebApplication.Models;
using K9.Base.Globalisation;

namespace K9.Base.WebApplication.Options
{
	public class BreadCrumbsOptions
	{
		private readonly List<Crumb> _crumbs;
		private readonly ViewContext _viewContext;

		public BreadCrumbsOptions(List<Crumb> crumbs, ViewContext viewContext)
		{
			_viewContext = viewContext;
			_crumbs = crumbs;
		}

		public BreadCrumbsOptions(ViewContext viewContext)
		{
			_viewContext = viewContext;
			_crumbs = new List<Crumb>();
		}

		public List<Crumb> Crumbs => GetCrumbs();

	    private List<Crumb> GetCrumbs()
		{
			var crumbs = new List<Crumb>();

			var homeCrumb = new Crumb
			{
				IsHome = true,
				Label = Dictionary.Home,
				ControllerName = "Home",
				ActionName = "Index",
			};

			homeCrumb.IsActive = IsCrumbActive(homeCrumb);
			crumbs.Add(homeCrumb);
			crumbs.AddRange(_crumbs);

			if (!crumbs.Any(c => c.IsActive))
			{
				var activeCrumb = new Crumb
				{
					Label = _viewContext.ViewBag.SubTitle ?? _viewContext.ViewBag.Title,
					ControllerName = GetActiveControllerName(),
					ActionName = GetActiveActionName(),
					IsActive = true
				};
				crumbs.Add(activeCrumb);
			}
			return crumbs;
		}

		private string GetActiveControllerName()
		{
			return _viewContext.RouteData.Values["controller"].ToString();
		}

		private string GetActiveActionName()
		{
			return _viewContext.RouteData.Values["action"].ToString();
		}

		private bool IsCrumbActive(Crumb crumb)
		{
			return
				GetActiveControllerName() == crumb.ControllerName &&
				GetActiveActionName() == crumb.ActionName;

		}
	}
}