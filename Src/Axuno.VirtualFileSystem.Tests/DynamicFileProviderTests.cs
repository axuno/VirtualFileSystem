﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;

namespace Axuno.VirtualFileSystem.Tests;

[TestFixture]
public class DynamicFileProviderTests
{
    private readonly IDynamicFileProvider _dynamicFileProvider;

    public DynamicFileProviderTests()
    {
        _dynamicFileProvider =
            ServiceSetup.GetVirtualFileSystemServiceProvider().GetRequiredService<IDynamicFileProvider>();
    }

    [Test]
    public void Should_Get_Created_Files()
    {
        const string fileContent = "Hello World";

        _dynamicFileProvider.AddOrUpdate(
            new InMemoryFileInfo(
                "/my-files/test.txt",
                fileContent.GetBytes(),
                "test.txt"
            )
        );

        var fileInfo = _dynamicFileProvider.GetFileInfo("/my-files/test.txt");
        Assert.That(fileInfo, Is.Not.Null);
        Assert.That(fileInfo.ReadAsString(), Is.EqualTo(fileContent));
    }
        
    [Test]
    public void Should_Create_and_Delete_Files()
    {
        const string fileContent = "Hello World";
        const string dynamicPath = "/to-delete/test.txt";
            
        _dynamicFileProvider.AddOrUpdate(
            new InMemoryFileInfo(
                dynamicPath,
                fileContent.GetBytes(),
                "test.txt"
            )
        );
            
        // Register to change on that file

        var fileCallbackCalled = false;

        ChangeToken.OnChange(
            () => _dynamicFileProvider.Watch(dynamicPath),
            () => { fileCallbackCalled = true; });

        var fileInfo = _dynamicFileProvider.GetFileInfo(dynamicPath);
        Assert.That(fileInfo.Exists);

        // Deleting the file should trigger the callback
            
        _dynamicFileProvider.Delete(dynamicPath);
        fileInfo = _dynamicFileProvider.GetFileInfo(dynamicPath);
        Assert.That(fileInfo.Exists, Is.False);

        Assert.That(fileCallbackCalled);
    }

    [Test]
    public void Deleting_Non_Existing_File_Just_Fails()
    {
        Assert.That(_dynamicFileProvider.Delete("does-not-exist"), Is.False);
    }
        
    [Test]
    public void Should_Get_Notified_On_File_Change()
    {
        // Create a dynamic file

        _dynamicFileProvider.AddOrUpdate(
            new InMemoryFileInfo(
                "/my-files/test.txt",
                "Hello World".GetBytes(),
                "test.txt"
            )
        );

        // Register to change on that file

        var fileCallbackCalled = false;

        ChangeToken.OnChange(
            () => _dynamicFileProvider.Watch("/my-files/test.txt"),
            () => { fileCallbackCalled = true; });

        // Updating the file should trigger the callback

        _dynamicFileProvider.AddOrUpdate(
            new InMemoryFileInfo(
                "/my-files/test.txt",
                "Hello World UPDATED".GetBytes(),
                "test.txt"
            )
        );

        Assert.That(fileCallbackCalled);
            
        // Updating the file should trigger the callback (2nd test)

        fileCallbackCalled = false;

        _dynamicFileProvider.AddOrUpdate(
            new InMemoryFileInfo(
                "/my-files/test.txt",
                "Hello World UPDATED 2".GetBytes(),
                "test.txt"
            )
        );

        Assert.That(fileCallbackCalled);
    }
}
