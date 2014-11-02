using Newtonsoft.Json;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch
{
    public class IndexDefinition
    {
        public string Name = Settings.IndexName;
        public IndexField[] Fields =
        { 
            //Adding the fields that the index will contain. 
            new IndexField() { Name = "contentId", Key = true, Searchable = false},
            new IndexField() { Name = "contentType", Searchable = false},
            new IndexField() { Name = "name", Suggestions = true},
            new IndexField() { Name = "url", Searchable = false, Filterable = false, Sortable = false, Facetable = false},
            new IndexField() { Name = "title", Suggestions = true, UmbracoProperty = "title"},
            new IndexField() { Name = "subHeading", UmbracoProperty = "subheader"},
            new IndexField() { Name = "mainContent", UmbracoProperty = "bodyText"},
        };

        //Scoring profile that boosts "Name" and "Title"
        public object[] ScoringProfiles = { new
        {
            Name = "boostNameAndTitle",
            Text = new { Weights = new { Name = 3, Title = 2 }}
        }};
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