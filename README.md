<img src="https://raw.githubusercontent.com/axuno/Axuno.VirtualFileSystem/main/VirtualFileSystem.png" width="64" alt="Logo">

# Axuno.VirtualFileSystem

The Virtual File System makes it possible to manage files that do not exist on a physical file system (e.g. disk).

* The ```VirtualFileSystem``` can be extended by additional ```IVirtualFileProvider```s.
* Out-of-the-box, ```Microsoft.Extensions.FileProviders.Composite```, ```Microsoft.Extensions.FileProviders.Embedded``` and ```Microsoft.Extensions.FileProviders.Physical``` are integrated.
* Virtual files can be used just like static files in an application.
* JavaScript, CSS, image files and all other file types can be embedded into assemblies and used just like the static files.
* An application (or library) can override an embedded file just by placing a static file with the same name and extension into the same folder of the virtual file system.


The library is a modified version of [Volo.Abp.VirtualFileSystem](https://github.com/abpframework/abp/tree/dev/framework/src/Volo.Abp.VirtualFileSystem) 3.3.1
Modifications to the source code were made by axuno in 2020. Changes focused on:

* Decouple Volo.Abp.VirtualFileSystem from all dependencies of the Abp Framework
* Use Microsoft DependencyInjection instead of [AutoFac](https://autofac.org/)
* Add a workaround, so that ```VirtualFileProvider``` will also find existing directories returned from ```PhysicalFileProvider```. This means, that ```VirtualFileProvider``` behaves the same, never mind whether files are retrieved using ```EmbeddedFileProvider``` or ```PhysicalFileProvider```.
* Change of namespaces

### Get started
[![NuGet](https://img.shields.io/nuget/v/Axuno.VirtualFileSystem.svg)](https://www.nuget.org/packages/Axuno.VirtualFileSystem/) Install the NuGet package
