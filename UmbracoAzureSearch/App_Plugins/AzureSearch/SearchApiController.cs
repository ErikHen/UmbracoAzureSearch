using System.Collections.Generic;
using System.Web.Http;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    [PluginController("AzureSearch")]
    public class SearchApiController : UmbracoApiController
    {
        [HttpGet]
        public List<string> Suggest(string term)
        {
           return AzureSearchHelper.Suggest(term);
        }
    }
}