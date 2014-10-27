using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public class RegisterEvents : IApplicationEventHandler
    {
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
            //TODO: AzureSearchHelper.AddContentToIndex(e.);
        }

        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentService_Published;
            ContentService.UnPublished +=ContentService_UnPublished;
            ContentService.Deleted +=ContentService_Deleted;
            ContentService.Moved += ContentService_Moved;
            //ContentService.Trashed += ContentService_Trashed; //TODO
            //TODO when user moves page from trash it should be reindexed..?
            //TODO when user moves page it should be reindexed..?

        }

        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
    
}


