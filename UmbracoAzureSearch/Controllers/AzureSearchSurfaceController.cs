using System.Web.Mvc;
using Umbraco.Web.Mvc;
using UmbracoAzureSearch.App_Plugins.AzureSearch;
using UmbracoAzureSearch.Models;

namespace UmbracoAzureSearch.Controllers
{
    public class AzureSearchSurfaceController : SurfaceController
    {
        /// <summary>
        /// Renders the Contact Form
        /// @Html.Action("RenderSearchForm","AzureSearchSurface");
        /// </summary>
        /// <returns></returns>
        public ActionResult RenderSearchForm()
        {
            //If TempData["AzureSearchViewModel"] has a value, it means that this is the request directly after a post to HandleSearchForm
            var viewModel = TempData["AzureSearchViewModel"] ?? new AzureSearchViewModel();

            //Return the partial view AzureSearch.cshtml in /Views/Partials/
            return PartialView("AzureSearch", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult HandleSearchForm(AzureSearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return CurrentUmbracoPage();
            }

            string searchPhrase = model.SearchPhrase;
            // If blank search, assume they want to search everything
            if (string.IsNullOrWhiteSpace(searchPhrase))
            { 
                searchPhrase = "*";
            }
            model.SearchResult = AzureSearchHelper.Search(searchPhrase);

            //Set the data that will be used in the view
            TempData["AzureSearchViewModel"] = model;

            //All done - redirect to the current page
            return RedirectToCurrentUmbracoPage();
        }
    }
}