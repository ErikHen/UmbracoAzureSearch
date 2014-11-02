using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public static class AzureSearchHelper
    {
        private static readonly JsonSerializerSettings JsonSettings;

        static AzureSearchHelper()
        {
            JsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            JsonSettings.Converters.Add(new StringEnumConverter());
        }

        public static string SerializeJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSettings);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSettings);
        }

        public static HttpResponseMessage SendSearchRequest(HttpMethod method, Uri uri, string json = null)
        {
            var httpClient = new HttpClient(); 
            httpClient.DefaultRequestHeaders.Add("api-key", Settings.ApiKey);

            var builder = new UriBuilder(uri);
            string separator = string.IsNullOrWhiteSpace(builder.Query) ? string.Empty : "&";
            builder.Query = builder.Query.TrimStart('?') + separator + Settings.ApiVersion;

            var request = new HttpRequestMessage(method, builder.Uri);

            if (json != null)
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            return httpClient.SendAsync(request).Result;
        }

        public static void EnsureSuccessfulSearchResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string error = response.Content == null ? null : response.Content.ReadAsStringAsync().Result;
                throw new Exception("Search request failed: " + error);
            }
        }

        #region Index operations
        private static bool IndexExists()
        {
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName);
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Get, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            response.EnsureSuccessStatusCode();
            return true;
        }

        public static dynamic GetIndex()
        {
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName);
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Get, uri);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                return null;
            }
        }

        public static dynamic GetIndexStatistics()
        {
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName + "/stats");
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Get, uri);
            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject(response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                return null;
            }
        }

        public static string CreateIndex()
        {
            //Some changes are possible to do to an existing index, but others need a complete whipe of index first.
            DeleteIndex();
            
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName);
            string json = AzureSearchHelper.SerializeJson(new IndexDefinition());
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Put, uri, json);

            string successMessage = "Index created/updated successfully";
            if (!response.IsSuccessStatusCode)
            {
                successMessage = response.Content == null ? null : response.Content.ReadAsStringAsync().Result;
            }
            return successMessage;
        }

        private static bool DeleteIndex()
        {

            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName);
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Delete, uri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
            response.EnsureSuccessStatusCode();
            return true;
        }
        #endregion

        #region Add/delete content in index
        /// <summary>
        /// Add content to index. Udates index if content already exists in index.
        /// </summary>
        /// <param name="addEntities"></param>
        public static void AddContentToIndex(IEnumerable<IContent> addEntities)
        {
            var documentsJson = new StringBuilder();
            var indexFields = new IndexDefinition().Fields;
            foreach (var content in addEntities.Take(1000)) //TODO: handle more than 1000 (do multiple service requests)
            {
                documentsJson.Append(CreateUploadDocumentJson(content, indexFields) + ",");
            }

            string json = "{ value: [ " + documentsJson.ToString().TrimEnd(',') + " ]}";
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName + "/docs/index");
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Post, uri, json);

            //TODO: The response can be "partial success". Some docs have been indexed, but others have failed. Needs to be handled(?).
            //if (!response.IsSuccessStatusCode)
            //{
            //    //TODO: Log this or something string error = response.Content == null ? null : response.Content.ReadAsStringAsync().Result;
            //}
            
            }

        public static void DeleteContentFromIndex(IEnumerable<IContent> deleteEntities)
        {
            var documentsJson = new StringBuilder();
            foreach (var content in deleteEntities.Take(1000)) //TODO: handle more than 1000 (do multiple service requests)
            {
                documentsJson.Append(CreateDeleteDocumentJson(content) + ",");
            }

            string json = "{ value: [ "+ documentsJson.ToString().TrimEnd(',') +" ]}";
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName + "/docs/index");
            HttpResponseMessage response = SendSearchRequest(HttpMethod.Post, uri, json);

            //if (!response.IsSuccessStatusCode)
            //{
            //    //TODO: Log this or something
            //}

        }

        public static string CreateDeleteDocumentJson(IContent content)
        {
            string documentJson = "{ @search.action: \"delete\", contentId: \"" + content.Id + "\" }";
            return documentJson;
        }

        public static string CreateUploadDocumentJson(IContent content, IndexField[] indexFields)
        {
            string documentJson = "{ contentId: \"" + content.Id + "\"," +
                                  "name: \"" + content.Name + "\"," +
                                  "contentType: \"" + content.ContentType.Alias + "\"," +
                                  "url: \"" + new UmbracoHelper().NiceUrl(content.Id) + "\",";

            //The index fields specifies which properties that should be indexed 
            var fieldsThatMapToAnUmbracoProperty = indexFields.Where(f => !string.IsNullOrEmpty(f.UmbracoProperty));
            foreach (var field in fieldsThatMapToAnUmbracoProperty)
            {
                //Get property value (if property exists and has a value)
                var property = content.Properties.SingleOrDefault(p => p.Alias == field.UmbracoProperty);
                if (property != null)
                {
                    if (property.Value != null)
                    {
                        documentJson += field.Name + ": " + SerializeJson(property.Value.ToString().StripHtml()) + ",";
                    }
                }
                
            }

            documentJson = documentJson.TrimEnd(',');
            documentJson += "}";
            return documentJson;
        }
        #endregion

        #region Search 
        public static dynamic Search(string searchPhrase)
        {
            string search = "&search=" + Uri.EscapeDataString(searchPhrase);
            //string facets = "&facet=color&facet=categoryName&facet=listPrice,values:10|25|100|500|1000|2500";
            //string paging = "&$top=10";
            //string filter = BuildFilter(color, category, priceFrom, priceTo);
            //string orderby = BuildSort(sort);
            //TODO: start using scoring profile

            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName + "/docs?$count=false" + search);
            HttpResponseMessage response = SendSearchRequest(HttpMethod.Get, uri);
            EnsureSuccessfulSearchResponse(response);

            return DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
        }

        public static List<string> Suggest(string searchPhrase)
        {
            var uri = new Uri(Settings.ServiceUri, "/indexes/" + Settings.IndexName + "/docs/suggest?fuzzy=true&search=" + Uri.EscapeDataString(searchPhrase));
            HttpResponseMessage response = AzureSearchHelper.SendSearchRequest(HttpMethod.Get, uri);
            EnsureSuccessfulSearchResponse(response);

            var result = DeserializeJson<dynamic>(response.Content.ReadAsStringAsync().Result);
            var options = new List<string>();
            foreach (var option in result.value)
            {
                options.Add((string)option["@search.text"]);
            }

            return options;
        }
        #endregion
    }
}
