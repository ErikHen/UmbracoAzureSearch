Umbraco + Azure Search
==================

A demo of how to Use Azure Search as search engine for an Umbraco site. 
When publishing a page in Umbraco, the content on that page will be pushed to the Azure Search service to be index. 
Users can enter a search phrase that gets sent to the searh service, and a search result is returned. 

Why?
=======================
Why would you use Azure Search instead of Examine for Umbraco? I don't really know :-), I have to little knowledge about Examine to really compare the two. But it seems to be a bit tricky to get Examine to work when hosting on Azure(?), and if using a load balanced environment it's even more tricky. 
But I do know that Azure Search is quite easy to set up and use.

I created this project because I wanted to try out Azure Search + learn more about Umbraco. This code was produced for testing/demo purposes, but I guess that it could be the base for something else.

How to try it out
=========================
Create an Azure Search service (http://azure.microsoft.com/en-us/documentation/services/search/) Note the service url, index name and Api key for your service/index.

Install a new Umbraco site v 7.1.???, Select the “Txt” starter website

Copy all files
Add reference to (Assemblies->Framework) System.Net.Http

Change ServiceUri + indexname + ApiKey in App_Plugin/AzureSearch/Settings.cs


Change settings for your user in Backoffice. Give user access to section “[AzureSearch]”. Might need to reaload. Now you should see a new section in the left menu.

Note that some of the fields in the index are specific to Txt starter website, change it in Indexdefinition.cs

TODO:  Add more info here..
