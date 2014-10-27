using System.Net.Http.Formatting;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Mvc;
using Umbraco.Web.Trees;

namespace UmbracoAzureSearch.App_Plugins.AzureSearch.Backoffice.AzureSearchTree
{
    [PluginController("AzureSearch")]
    [Umbraco.Web.Trees.Tree("AzureSearch", "AzureSearchTree", "Azure Search", iconClosed: "icon-search")]
    public class AzureSearchTreeController : TreeController
    {
        protected override TreeNodeCollection GetTreeNodes(string id, FormDataCollection queryStrings)
        {
            var nodes = new TreeNodeCollection();
            if (id == "-1") // root
            {
                var item = this.CreateTreeNode("dashboard", id, queryStrings, "Search index", "icon-search", true);
                nodes.Add(item);
            }
            return nodes;
        }

        protected override MenuItemCollection GetMenuForNode(string id, FormDataCollection queryStrings)
        {
            var menu = new MenuItemCollection();
            return menu;
        }
    }
}