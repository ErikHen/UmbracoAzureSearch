# Umbraco + Azure Search
A demo of how to Use Azure Search as search engine for an Umbraco site.

* When publishing a page in Umbraco, the content on that page will be pushed to the Azure Search service to be indexed. 
* Users can enter a search phrase that gets sent to the searh service, and a search result is returned. 

## Why?
I created this project because I wanted to try out Azure Search + learn more about Umbraco. This code was produced for testing/demo purposes, but I guess that it could be the base for something else.

So why would you use Azure Search instead of Examine for Umbraco? I don't really know :-), I have to little knowledge about Examine to really compare the two. But it seems to be a bit tricky to get Examine to work when hosting on Azure(?), and if using a load balanced environment it's even more tricky. 
But I do know that Azure Search is quite easy to set up and use, and has some nice features even though it's still in preview.

## How to try it out
First set everything up
* [Create an Azure Search service](http://azure.microsoft.com/en-us/documentation/services/search/) Note the service url, index name and Api key for your service/index.
* Install a new Umbraco site (or use an existing one). I've used v7.1.6 and v7.1.8 and the “Txt” starter website.
* Copy all files from the UmbracoAzureSearch folder to the root of your site. You may need to add reference to System.Net.Http for your project to build.
* Change ServiceUri + indexName + ApiKey in App_Plugin/AzureSearch/Settings.cs to the values you have for your Azure Search service.
* Your Umbraco backoffice user needs to have access to section “[AzureSearch]”. Now you should see a new section in the left menu.
* Add the following to the main area of an existing (or new) template/view:
```
@Html.Action("RenderSearchForm", "AzureSearchSurface")
```

Now you're ready to start using it
* Start by creating a new index in the "[AzureSearch]" section.
* Publish all pages to have them sent to the search service for indexing.
* Go to a page where you added the rendering of the search form and search for something.

## Help wanted
I'm not super experienced in Umbraco or MVC. This is my first encounter with Angular.js, and my first GitHub project... So any comments or help of any kind is greatly appreciated.

## Disclaimer
This code is not function complete or well tested, and has never been near a production environment. The indexing currently only works with one language (english), multilanguage support will come, according to Microsoft.
