using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public class RegisterEvents : IApplicationEventHandler
    {
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentService_Published;
            ContentService.UnPublished += ContentService_UnPublished;
            ContentService.Deleted += ContentService_Deleted;
            ContentService.Moved += ContentService_Moved;
            ContentService.Trashed += ContentService_Trashed;
        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        void ContentService_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            AzureSearchHelper.AddContentToIndex(e.PublishedEntities);
        }

        void ContentService_UnPublished(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            AzureSearchHelper.DeleteContentFromIndex(e.PublishedEntities);
        }

        void ContentService_Deleted(IContentService sender, DeleteEventArgs<IContent> e)
        {
            AzureSearchHelper.DeleteContentFromIndex(e.DeletedEntities);
        }

        void ContentService_Moved(IContentService sender, MoveEventArgs<IContent> e)
        {
            //Get the entities that were moved, but only add them to index if they are published.
            var movedEntities = e.MoveInfoCollection.Where(eventInfo => eventInfo.Entity.Published).Select(eventInfo => eventInfo.Entity).ToList();
            AzureSearchHelper.AddContentToIndex(movedEntities);
        }

        void ContentService_Trashed(IContentService sender, MoveEventArgs<IContent> e)
        {
            var movedEntities = e.MoveInfoCollection.Select(eventInfo => eventInfo.Entity).ToList();
            AzureSearchHelper.DeleteContentFromIndex(movedEntities);
        }
    }
}


