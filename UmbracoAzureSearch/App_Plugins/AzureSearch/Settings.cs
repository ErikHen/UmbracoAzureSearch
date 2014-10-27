using System;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public static class Settings
    {
        public static Uri ServiceUri = new Uri("https://erik-dev.search.windows.net");
        public static string IndexName = "<my index name>";
        public static string ApiKey = "<my api key>";
        public static string ApiVersion = "api-version=2014-07-31-Preview";
    }
}