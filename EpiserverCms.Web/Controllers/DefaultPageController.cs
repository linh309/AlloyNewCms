using System;
using System.Web.Mvc;
using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Web.Mvc;
using EpiserverCms.Web.Models.Pages;
using EpiserverCms.Web.Models.ViewModels;

namespace EpiserverCms.Web.Controllers
{
    public interface IPlainPageService
    {
        object GetPropertyByName(int pageId, string propName);
    }

    public class PlainPageService : IPlainPageService
    {
        private IContentRepository _loader;
        public PlainPageService()
        {
            // _loader = loader;
        }

        public object GetPropertyByName(int pageId, string propName)
        {
            object value = null;

            var page = _loader.Get<SitePageData>(new PageReference(pageId));
            if (page.Property.Contains(propName))
            {
                value = page.Property[propName];
            }

            return value;
        }
    }

    /// <summary>
    /// Concrete controller that handles all page types that don't have their own specific controllers.
    /// </summary>
    /// <remarks>
    /// Note that as the view file name is hard coded it won't work with DisplayModes (ie Index.mobile.cshtml).
    /// For page types requiring such views add specific controllers for them. Alterntively the Index action 
    /// could be modified to set ControllerContext.RouteData.Values["controller"] to type name of the currentPage
    /// argument. That may however have side effects.
    /// </remarks>
    [TemplateDescriptor(Inherited = true)]
    public class DefaultPageController : PageControllerBase<SitePageData>
    {
        IPlainPageService _IContentRenderer;
        public DefaultPageController(IPlainPageService IContentRenderer)
        {
            _IContentRenderer = IContentRenderer;
        }

        public ViewResult Index(SitePageData currentPage)
        {
            var model = CreateModel(currentPage);

            return View(string.Format("~/Views/{0}/Index.cshtml", currentPage.GetOriginalType().Name), model);
        }

        /// <summary>
        /// Creates a PageViewModel where the type parameter is the type of the page.
        /// </summary>
        /// <remarks>
        /// Used to create models of a specific type without the calling method having to know that type.
        /// </remarks>
        private static IPageViewModel<SitePageData> CreateModel(SitePageData page)
        {
            var type = typeof(PageViewModel<>).MakeGenericType(page.GetOriginalType());
            return Activator.CreateInstance(type, page) as IPageViewModel<SitePageData>;
        }
    }
}
