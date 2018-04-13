using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EpiserverCms.Web.Models.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EpiserverCms.Web.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class CustomGroupPageInitialization : IInitializableModule
    {
        private string TopGroupPageName = "Articles";

        public void Initialize(InitializationEngine context)
        {
            //Regiser event
            var contentEvent = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvent.CreatingContent += Ce_CreatingContent;
        }

        public void Uninitialize(InitializationEngine context)
        {
            //uregister event
            var contentEvent = ServiceLocator.Current.GetInstance<IContentEvents>();
            contentEvent.CreatingContent -= Ce_CreatingContent;
        }

        private void Ce_CreatingContent(object sender, ContentEventArgs e)
        {
            var newPage = e.Content as PageData;
            var isNeedToGroup = IsAbleToGroup(newPage);

            if (isNeedToGroup)
            {                
                var year = DateTime.Now.AddYears(1).Year;
                var month = DateTime.Now.Month;
                var day = DateTime.Now.Day;
                var listGroups = new List<int> { year, month, day }.Select(g => g.ToString()).ToList();

                //1. get top group container page
                var topGroupContainerPage = GetTopGroupPageReference(TopGroupPageName);
                var currentParentGroup = topGroupContainerPage;

                foreach (var group in listGroups)
                {
                    var childPage = GetSpecifiedChildFromParent(currentParentGroup, group);
                    if (childPage != null)
                    {
                        currentParentGroup = childPage.ContentLink.ToPageReference();
                    }
                    else
                    {
                        childPage = DataFactory.Instance.GetDefault<ContainerPage>(currentParentGroup);
                        childPage.PageName = group;
                        DataFactory.Instance.Save(childPage, EPiServer.DataAccess.SaveAction.Publish);
                        currentParentGroup = childPage.ContentLink.ToPageReference();
                    }
                }

                newPage.ParentLink = currentParentGroup;
            }
        }

        /// <summary>
        /// Determine which page is the top group parent page of all group pages
        /// </summary>
        /// <returns></returns>
        private PageReference GetTopGroupPageReference(string topGroupPageName)
        {
            var topGroupPageContainer = PageReference.StartPage;
            if (!string.IsNullOrEmpty(topGroupPageName))
            {
                var topGroupPageData = GetSpecifiedChildFromParent(PageReference.StartPage, topGroupPageName);
                if (topGroupPageData != null)
                {
                    topGroupPageContainer = topGroupPageData.ContentLink.ToPageReference();
                }
                else
                {
                    //if top group page hasn't been existed => need to create a page CotainerPage type
                    ContainerPage topGroupContainerPage = DataFactory.Instance.GetDefault<ContainerPage>(PageReference.StartPage);
                    topGroupContainerPage.PageName = topGroupPageName;
                    DataFactory.Instance.Save(topGroupContainerPage, EPiServer.DataAccess.SaveAction.Publish);
                    topGroupPageContainer = topGroupContainerPage.ContentLink.ToPageReference();
                }
            }

            return topGroupPageContainer;
        }

        private PageData GetSpecifiedChildFromParent(ContentReference parent, string childName)
        {
            var children = DataFactory.Instance.GetChildren<PageData>(parent).ToList();
            var child = children.Where(p => p.Name.CompareTo(childName) == 0).FirstOrDefault();

            return child;
        }

        private bool IsAbleToGroup(IContent content)
        {
            return IsInPageDataNeedToGroup(content)
                && !(content is ContainerPage);
        }

        private bool IsInPageDataNeedToGroup(IContent content)
        {
            return content is ArticlePage
                    || content is NewsPage;
        }
    }
}