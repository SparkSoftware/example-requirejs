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

_NOTE: Thank you to [Yngve Bakken Nilsen](http://www.novanet.no/blog/yngve-bakken-nilsen/) for the idea of using text templates to generating the `build.js` file_

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
     
In addition to the above helper methods an additional property has been added `MainModule`  that will return the module name for the current view (i.e., _view/home/index/main_).

    <div data-require="@MainModule" />
    
## Main.js

The `main.js` file represents the core application module that will be loaded for every view. You may notice that several module names have been mapped using `*` rather than defining an explicit `path`. Given that module names are relative to the referenced module, if we aliased `lib/jquery/package` via a `path` entry to `pkg/jquery`, any relative module names like `./jquery.cookie` would be expected at `pkg/jquery/jquery.cookie` and not `lib/jquery/jquery.cookie`.

_NOTE: Packages will be discussed in more detail below._

**Example main.js**

    require.config({
        paths: {
            'jquery': ['//ajax.aspnetcdn.com/ajax/jQuery/jquery-1.11.0.min', 'vendor/jquery/jquery'],
            'knockout': ['//ajax.aspnetcdn.com/ajax/knockout/knockout-3.0.0', 'vendor/knockout/knockout'],
            'bootstrap': ['//ajax.aspnetcdn.com/ajax/bootstrap/3.1.0/bootstrap.min', 'vendor/bootstrap/bootstrap']
        },
        shim: {
            'bootstrap': ['jquery']
        },
        map: {
            '*': {
                'pkg/jquery': 'lib/jquery/package',
                'pkg/knockout': 'lib/knockout/package'
            }
        }
    });
    
    define(['config', 'pkg/jquery', 'pkg/knockout'], function() { });

_IMPORTANT: Any files that you wish to include in the core main module should be included in the dependency list of `define([~], function() {});` to ensure the modules will be bundled during the build process._

## Packages, Views & Build.tt

**Packages**

Often in an application you require a common set of libraries to be loaded. Perhaps your application will always use `jquery.validate` along with `jquery`. You could explicitly define each dependency for every module that requires the aforementioned modules; however this is tedious, error prone and very easy to forget. The example `build.tt` will scan the _lib_ folder structure for any `package.js` file and ensure that the _r.js_ optimizer will bundle all referenced files together. Optionally, any packaged files may then be referenced in `main.js` to bundle together the core application module (i.e., resulting in a single file where warranted).

_NOTE: The example application chooses to alias each package under a virtual folder `pkg` to ensure that a dependency on `jquery` can be easily distinguished from `pkg/jquery`._

**Views

Views follow a similar pattern to _packages_. The _view_ folder structure mimics the ASP.NET MVC convention for the most part. A view found at `views\home\index` will expect the main module to exist at `~/Content/js/view/home/index/main.js`. Why _main.js_ instead of _index.js_ you may ask? Two reasons, the first being that by using a folder to represent the view, all related views are kept organized. The second reason is that scanning for _main.js_ to find modules to optimize  is much nicer than trying to optmize down all files. 

** build.tt and build.js

The _build.tt_ text template is used to generate the _build.js_ requirejs optimization configuration file. The current implementation of _built.tt_ will bundle all modules referenced by main in to a single file that will _always_ be loaded for each view. All _packages_ will also be bundled in to a single file (and potentially included in main if referenced). Finally, all _main_ view files will be bundled together to ensure no more than two JavaScript file requests per page if desired.

_NOTE: The example project uses a CDN for jQuery, Knockout and Bootstrap resulting in three additional requests._
