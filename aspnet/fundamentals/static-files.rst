.. _fundamentals-static-files:

Working with Static Files
=========================
By `Rick Anderson`_

Static files, such as HTML, CSS, image, and JavaScript, are assets that an ASP.NET Core app can serve directly to clients.

.. contents:: Sections
  :local:
  :depth: 1

Serving static files
--------------------

Static files are typically located in the web root (*<content-root>/wwwroot*) folder. See Content root and Web root in  :doc:`/conceptual-overview/aspnet` for more information. You generally set the content root to be the current directory so that your project's webroot will be found while in development.

.. literalinclude:: ../../common/samples/WebApplication1/src/WebApplication1/Program.cs
  :language: c#
  :lines: 12-22
  :emphasize-lines: 5
  :dedent: 8


Static files can be stored in any folder under the *wwwroot* and accessed with a relative path to that root. For example, when you create a default Web application project using Visual Studio, there are several folders created within the *wwwroot*  folder - *css*, *images*, and *js*. The URI to access an image in the *images* subfolder:

- \http://<app>/images/<imageFileName>
- \http://localhost:9189/images/banner3.svg


In order for static files to be served, you must configure the :doc:`middleware` to add static files to the pipeline. The static file middleware can be configured by adding a dependency on the *Microsoft.AspNetCore.StaticFiles* package to your project and then calling the :dn:method:`~Microsoft.AspNetCore.Builder.StaticFileExtensions.UseStaticFiles` extension method from ``Startup.Configure``:

.. literalinclude:: static-files/sample/Startup.cs
  :language: c#
  :lines: 38-39,53-55
  :emphasize-lines: 5
  :dedent: 8

You must include "Microsoft.AspNetCore.StaticFiles" in the *project.json* file.

Suppose you have a project hierarchy where the static files you wish to serve are outside the webroot. For example:

  - wwwroot

    - css
    - images
    - ...

  - MyStaticFiles

    - test.png

For a request to access *test.png*, configure the static files middleware as follows:

.. literalinclude:: static-files/sample/Startup.cs
  :language: c#
  :lines: 38-39,53-62
  :emphasize-lines: 7-12
  :dedent: 8

A request to ``http://<yourApp>/StaticFiles/test.png`` will serve the *test.png* file.

Static file authorization
-------------------------

The static file module provides **no** authorization checks. Any files served by it, including those under *wwwroot* are publicly available. If you want to serve files based on authorization you should:

- Store them outside of *wwwroot* and any directory accessible to the static file middleware
- Serve them through a controller action, returning a :dn:class:`~Microsoft.AspNetCore.Mvc.FileResult` where authorization is applied

Enabling directory browsing
---------------------------

Directory browsing allows the user of your web app to see a list of directories and files within a specified directory. Directory browsing is disabled by default for security reasons. To enable directory browsing, call the ``UseDirectoryBrowser`` extension method from  ``Startup.Configure`` as follows:

.. literalinclude:: static-files/sample/Startup.cs
  :language: c#
  :lines: 38-39,53,54,64-69
  :emphasize-lines: 5-11
  :dedent: 8

The code above allows directory browsing of the *wwwroot/images* folder:

.. image:: static-files/_static/dir-browse.png

.. warning:: Enabling directory browsing can leak secrets. We recommend that you **not** enable directory browsing in production. Ensure that you only enable browsing on directories with no secrets or sensitive data.

Serving a default document
--------------------------

Setting a default home page gives site visitors a place to start when visiting your site. In order for your Web app to serve a default page without the user having to fully qualify the URI, call the ``UseDefaultFiles`` extension method from ``Startup.Configure`` as follows.

.. literalinclude:: static-files/sample/StartupEmpty.cs
  :language: c#
  :lines: 14-18
  :emphasize-lines: 3
  :dedent: 8

.. note:: :dn:method:`~Microsoft.AspNetCore.Builder.DefaultFilesExtensions.UseDefaultFiles` must be called before ``UseStaticFiles`` to serve the default file. ``UseDefaultFiles`` is a URL re-writer that doesn't actually serve the file. You must enable the static file middleware (``UseStaticFiles``) to serve the file.

With :dn:method:`~Microsoft.AspNetCore.Builder.DefaultFilesExtensions.UseDefaultFiles`, requests to a folder will search for:

  - default.htm
  - default.html
  - index.htm
  - index.html

The first file found from the list will be servered as if the request was the fully qualified URI (although the browser URL will continue to show the URI requested).

The following code shows how to change the default file name to *mydefault.html*.

.. literalinclude:: static-files/sample/StartupDefault.cs
  :language: c#
  :lines: 14-22
  :dedent: 8

UseFileServer
------------------------------

:dn:method:`~Microsoft.AspNetCore.Builder.FileServerExtensions.UseFileServer` combines the functionality of :dn:method:`~Microsoft.AspNetCore.Builder.StaticFileExtensions.UseStaticFiles`, :dn:method:`~Microsoft.AspNetCore.Builder.DefaultFilesExtensions.UseDefaultFiles`, and :dn:method:`~Microsoft.AspNetCore.Builder.DirectoryBrowserExtensions.UseDirectoryBrowser`.

The following code enables static files and the default file to be served, but does not allow directory browsing:

.. code-block:: c#

  app.UseFileServer();

The following code enables static files, default files and  directory browsing:

.. code-block:: c#

  app.UseFileServer(enableDirectoryBrowsing: true);

As with ``UseStaticFiles``, ``UseDefaultFiles``, and ``UseDirectoryBrowser``, if you wish to serve files that exist outside the webroot, you instantiate and configure an :dn:class:`~Microsoft.AspNetCore.Builder.FileServerOptions` object that you pass as a parameter to ``UseFileServer``. For example, given the following directory hierarchy in your Web app:

- wwwroot

  - css
  - images
  - ...

- MyStaticFiles

  - test.png
  - default.html

Using the hierarchy example above, you might want to enable static files, default files, and browsing for the ``MyStaticFiles`` directory. In the following code snippet, that is accomplished with a single call to :dn:class:`~Microsoft.AspNetCore.Builder.FileServerOptions`.

.. literalinclude:: static-files/sample/StartupUseFS.cs
  :language: c#
  :lines: 55-61
  :dedent: 12

Using the file hierarchy and code above:

==========================================  ===================================
URI                                         Response
==========================================  ===================================
\http://<app>/StaticFiles/test.png          StaticFiles/test.png
\http://<app>/StaticFiles                   MyStaticFiles/default.html
==========================================  ===================================

If no default named files are in the *MyStaticFiles* directory, \http://<app>/StaticFiles returns the directory listing with clickable links:

.. image:: static-files/_static/db2.png

Non-standard content types
--------------------------

The ASP.NET static file middleware understands almost 400 known file content types. If the user requests a file of an unknown file type, the static file middleware returns a HTTP 404 (Not found) response. If directory browsing is enabled, a link to the file will be displayed, but the URI will return an HTTP 404 error.

The following code enables serving unknown types and will render the unknown file as an image. See  this `list of common MIME content types <http://www.iana.org/assignments/media-types/media-types.xhtml>`__.

.. literalinclude:: static-files/sample/StartupUnknown.cs
  :language: c#
  :lines: 55-66
  :dedent: 12

With the code above, a request for a file with an unknown content type will be returned as an image.

.. warning:: Enabling :dn:property:`~Microsoft.AspNetCore.Builder.StaticFileOptions.ServeUnknownFileTypes` is a security risk and using it is discouraged.  :dn:class:`~Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider`  (explained below) provides a safer alternative to serving files with non-standard extensions.

FileExtensionContentTypeProvider
^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

The :dn:class:`~Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider` class contains a  collection that maps file extensions to MIME content types. In the following sample, several file extensions are registered to known MIME types.

.. literalinclude:: static-files/sample/StartupFECTP.cs
  :language: c#
  :lines: 58-70
  :dedent: 12


Considerations
^^^^^^^^^^^^^^^^

- ASP.NET Core applications hosted in IIS use the ASP.NET Core Module to forward all requests to the application including requests for static files. The IIS static file handler is not used because it doesn't get a chance to handle requests before they are handled by the ASP.NET Core Module.

- Code files (including c# and Razor) should be placed outside of the app project's webroot (*wwwroot* directory). This creates a clean separation between your app's static (non-compliable) content and source code and protects source code from being leaked.


Additional Resources
--------------------

- :doc:`middleware`
