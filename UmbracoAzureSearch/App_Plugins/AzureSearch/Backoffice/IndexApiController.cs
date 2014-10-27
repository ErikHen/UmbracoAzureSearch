using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch.Backoffice
{
    // Since this is an AuthorizedApiController the route for this controller is /umbraco/backoffice/AzureSearch/Index/{action}

    [PluginController("AzureSearch")]
    public class IndexApiController : UmbracoAuthorizedApiController
    {
        [HttpGet]
        public string CreateIndex()
        {
           return AzureSearchHelper.CreateIndex();
        }

        [HttpGet]
        public object GetStatus()
        {
            var status = new
            {
                Info = "", //TODO
                Index = AzureSearchHelper.GetIndex(),
                Statistics = AzureSearchHelper.GetIndexStatistics()
            };
            return status;
        }

     
    }
}