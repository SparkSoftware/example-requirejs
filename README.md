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

_NOTE: If the current view model is `view/home/index` then the returned module will be `view/home/anotherModule`_
     
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

**Views**

Views follow a similar pattern to _packages_. The _view_ folder structure mimics the ASP.NET MVC convention for the most part. A view found at `views\home\index` will expect the main module to exist at `~/Content/js/view/home/index.js`.

**Build.tt and Build.js**

The _build.tt_ text template is used to generate the _build.js_ requirejs optimization configuration file. The current implementation of _built.tt_ will bundle all modules referenced by main in to a single file that will _always_ be loaded for each view. All _packages_ will also be bundled in to a single file (and potentially included in main if referenced). Finally, all _view_ files will be bundled together to ensure no more than two JavaScript file requests per page if desired.

_NOTE: The example project uses a CDN for jQuery, Knockout and Bootstrap resulting in three additional requests. Any view file prefixed with a underscore will be excluded from the build.js configuration._

## Build Process

The great thing about RequireJS is that out of the box, no build action is required. When ready to move your application in to your QA environment (or local test environment) you can simply run `publish.cmd` that will compile, minify and combine your JavaScript modules. 

_NOTE: The example build script will copy back the versioned content files to the project folder for convenience. Normally this additional copy task would not be required._

The published files may be found in the root `dist` folder. The un-optimized content files may be found in the `Content` folder. The optimized content files will be placed in a versioned content folder similar to `Content-0.0.0.0`. A versioned content folder is used as a cache busting mechanism. Although one could use [urlArgs](http://requirejs.org/docs/api.html#config-urlArgs) this has several undesirable side-effects. Most notably, if urlArgs is used, your CDN files will also be cache busted. In addition, some third-party libraries will pass down query string parameters resulting in unexpected side effects (i.e., MapQuest -- preventing map tiles from being cached).

Given that content may exist in `Content` for development and debugging and in `Content-X.X.X.X` for production, several [UrlExtensions](https://github.com/SparkSoftware/Spark.Examples.RequireJs/blob/master/src/Example/Controllers/UrlExtensions.cs) exist to abstract away the underlying file source. 

* _GetScriptBase()_ - Returns the root path to the script modules (i.e., _~/Content/js/_).
* _Script(relativePath)_ - Returns a script path (i.e., `Url.Script("lib\module.js")` maps to _~/Content/js/lib/main.js_).
* _StyleSheet(relativePath)_ - Returns a stylesheet path (i.e., `Url.Script("example.css")` maps to _~/Content/css/example.css_).
* _Image(relativePath)_ - Returns an image path (i.e., `Url.Image("sample.png")` maps to _~/Content/img/sample.png_).

## Referencing Modules

**Require Module(s)**

In order to wire up view specific RequireJS dependencies you may explicitly include the required script as follows:

    @section scripts
    {
        <script type="text/javascript" src="@Url.Script(MainModule + ".js")"></script>
    }

An alternate approach that was first introduced by a colleague [Simon Green](http://captaincodeman.com/) leverages custom data attributes. The approach used in the example extends the concept originally introduced by [Simon Green](http://captaincodeman.com/) by adding a separate `data-require` attribute and honoring the DOM's hierarchical structure. 

    <div data-require="@MainModule" />
    
or
    
    <div data-require="MyModule1, MyModule2" />
    
**Knockout Models**

If you use [KnockoutJS](http://knockoutjs.com/) an additional data attribute may be used to wire-up your knockout view models via the `data-model` attribute.

    <div data-model="@MainModule">
        <h3 data-bind="text: description"></h3>
        <p data-bind="text: additionalInformation"></p>
    </div>

_NOTE: The reference module must return the view model to bind to the DOM._

**Hierarchical Dependencies**

Finally, you may choose to leverage the hierarchical nature of the DOM to ensure dependencies are loaded in a specific order. 

    <div data-require="@MainModule">
    
        <address data-model="@Module("address")">
            <span data-bind="text: street1"></span><br />
            <span data-bind="text: street2"></span><br />
            <abbr title="Phone">P:</abbr><span data-bind="text: phone"></span>
        </address>
    
        <address data-model="@Module("email")">
            <strong>Support:</strong> <a href="#" data-bind="text: support, attr: { href: 'mailto:' + support() }"></a><br />
            <strong>Marketing:</strong> <a href="#" data-bind="text: marketing, attr: { href: 'mailto:' + marketing() }"></a>
        </address>
    
    </div>
    
_NOTE:  This is an atypical use case and you should evaluate your design to see if truly warranted. In the above example, the main module is referenced first to ensure that when optimized, the explicit model modules have already been loaded. Typically your main module would return both models and this would not be required._
