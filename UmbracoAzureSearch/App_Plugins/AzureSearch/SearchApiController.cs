using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    //The route for this controller: /umbraco/AzureSearch/SearchApi/{action}
    [PluginController("AzureSearch")] 
    public class SearchApiController : UmbracoApiController
    {
        [HttpGet]
        public List<object> Suggest(string term)
        {
           return AzureSearchHelper.Suggest(term);
        }
    }
}