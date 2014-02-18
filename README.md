ASP.NET MVC RequireJS Example
========================================

An example ASP.NET MVC multi-page application with RequireJS and build optimization.

## Prerequisites

* [Node.js](http://nodejs.org/download/)
* [Visual Studio 2013](http://www.visualstudio.com/downloads/download-visual-studio-vs)
* [Microsoft Visual Studio 2013 SDK](http://www.microsoft.com/en-ca/download/details.aspx?id=40758)
* [Modeling SDK for Microsoft Visual Studio 2013](http://www.microsoft.com/en-us/download/details.aspx?id=40754)

## Text Template Setup

The example solution uses a text template to automatically generate the `build.js` configuration file. Although you can manually compile a text template by right clicking on the text template `build.tt` and selecting `Run Custom Tool`, the example solution automatically re-generates the text template output file on each project build.

see: [Code Generation in a Build Process](http://msdn.microsoft.com/en-us/library/ee847423.aspx)

Two modifications must be made to your ASP.NET MVC project file as outlined in the aforementioned link:

1. Add the text template target line shown below

    `<Import Project="$(VSToolsPath)\TextTemplating\Microsoft.TextTemplating.targets" />`

    after either

    `<Import Project="$(MSBuildToolsPath)\Microsoft.CSharp.targets" />`

    or

    `<Import Project="$(MSBuildToolsPath)\Microsoft.VisualBasic.targets" />`

2. Add the following two additional properties to the project properties in the `.csproj` file. 

    `<TransformOnBuild>true</TransformOnBuild>`

    `<TransformOutOfDateOnly>false</TransformOutOfDateOnly>`

_NOTE: Thank you to [Yngve Bakken Nilsen](http://www.novanet.no/blog/yngve-bakken-nilsen/) for the idea of using a text template for generating the `build.js` file_

see: [Making RequireJS play nice with ASP.NET MVC](http://www.novanet.no/blog/yngve-bakken-nilsen/dates/2013/6/making-requirejs-play-nice-with-aspnet-mvc/)

## JavaScript Folder Structure

All static resources are contained within the `~/Content/` folder. Following the ASP.NET MVC style conventions, the core JavaScript folder has been broken down as follows:

* _lib_ - Represents application specific shared modules (i.e., common modules).
* _vendor_ - Represents third-party libraries (i.e., jQuery, knockout, etc.).
* _view_ - ASP.NET MVC View Folder structure.

Two additional files exist at `~/Content/js/`:

* _main.js_ - The core RequireJS configuration and `main` module definition.
* _config.js_ - Global configuration settings that can be accessed via `config` module dependency.

## Custom WebViewPage

A custom WebViewPage has been created to simplify the process of referencing modules and allow passing of configuration data in to the shared `config` module.

see: [RequireViewPage](https://github.com/SparkSoftware/Spark.Examples.RequireJs/blob/master/src/Example/Controllers/RequireViewPage.cs)

The RequireViewPage exposes two helper functions for use by all views 

**RenderModuleConfig()**

Renders out a script tag prior to the `require.js` script include that sets the RequireJS [baseUrl](http://requirejs.org/docs/api.html#config-baseUrl) as well as pass in any global configuration that may be required by the application.

    var require = {"baseUrl":"/Content/js/","config":{"config":{"baseUrl":"/","consoleEnabled":true}}}

**Module()**

Renders out a module name based on the relative path to the current view model.

    <div data-require="@Module("anotherModule")" />

_NOTE: If the current view model is `view/home/index/main` then the returned module will be `view/home/index/anotherModule`_
     
In addition to the above helper methods an additional property has been added `MainModule`  that will return the module name for the current view (i.e., _view/home/index/main).

    <div data-require="@MainModule" />
    
