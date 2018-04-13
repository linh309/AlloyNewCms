using EPiServer.Core;
using EPiServer.Data;
using EPiServer.Data.Dynamic;
using EPiServer.Personalization;
using EPiServer.Web.Mvc;
using EpiserverCms.Web.Models.Blocks;
using EpiserverCms.Web.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EpiserverCms.Web.Controllers
{

    public class PersonWithIDynamicData : IDynamicData
    {
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Identity Id { get; set; }
    }

    public class CommentUserBlockController : BlockController<CommentUserBlock>
    {
        // GET: CommentUserBlock
        public override ActionResult Index(CommentUserBlock currentBlock)
        {
            EPiServerProfile currentUser = EPiServerProfile.Current;
            var model = new CommentUserViewModel { Body = currentBlock.Body, User = currentUser.UserName };
            //PageHelper => get current page

            /*
             *        DynamicDataStore store = DynamicDataStoreFactory.Instance.CreateStore("People", typeof(Person));
            Identity id = store.Save(p);
             */

            

            var pageRouteHelper = EPiServer.ServiceLocation.ServiceLocator.Current.GetInstance<EPiServer.Web.Routing.IPageRouteHelper>();
            var pageReference = pageRouteHelper.PageLink;
            var id = pageReference.ID;

            PersonWithIDynamicData newPerson = new PersonWithIDynamicData
            {
                Name = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.Now
            };

            DynamicDataStore store = DynamicDataStoreFactory.Instance.CreateStore("Comment", typeof(PersonWithIDynamicData));
            var loadedPerson = store.LoadAll<PersonWithIDynamicData>();

            //var id = store.Save(newPerson);

            return PartialView(model);
        }
    }
}