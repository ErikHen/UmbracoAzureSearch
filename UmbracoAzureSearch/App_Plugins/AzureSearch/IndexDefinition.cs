using Newtonsoft.Json;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public class IndexDefinition
    {
        public string Name = Settings.IndexName;
        public IndexField[] Fields =
        { 
            //Adding the fields that the index will contain. 
            new IndexField() { Name = "ContentId", Key = true, Searchable = false},
            new IndexField() { Name = "ContentType", Searchable = false},
            new IndexField() { Name = "Name", Suggestions = true},
            new IndexField() { Name = "Url", Searchable = false, Filterable = false, Sortable = false, Facetable = false},
            new IndexField() { Name = "Title", Suggestions = true, UmbracoProperty = "title"},
            new IndexField() { Name = "SubHeading", UmbracoProperty = "subheader"},
            new IndexField() { Name = "MainContent", UmbracoProperty = "bodyText"},
        };

        ////TODO: Scoring profile that boosts "Name" and "Title"
    }

    public class IndexField
    {
        public IndexField()
        {
            Type = "Edm.String";
            Searchable = Retrievable = true;
            Filterable = Sortable = Facetable = Suggestions = false;
        }

        public string Name { get; set; }
        public string Type { get; set; }
        public bool Key { get; set; }
        public bool Searchable { get; set; }
        public bool Filterable { get; set; }
        public bool Sortable { get; set; }
        public bool Facetable { get; set; }
        public bool Retrievable { get; set; }
        public bool Suggestions { get; set; }

        [JsonIgnore] //Not part of the actual index, this property maps an Umbraco property to an index field
        public string UmbracoProperty { get; set; }
    }
}