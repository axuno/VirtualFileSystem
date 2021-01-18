using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Axuno.VirtualFileSystem.Tests
{
    class ServiceSetup
    {
        public static ServiceProvider GetVirtualFileSystemServiceProvider()
        {
            return new ServiceCollection()
                .Configure<VirtualFileSystemOptions>(options =>
                {
                    options.FileSets.AddEmbedded<ServiceSetup>("Axuno.VirtualFileSystem.Tests.Assets");
                    options.FileSets.AddPhysical(
                        Path.Combine(DirectoryLocator.GetTargetProjectPath(typeof(ServiceSetup)), "Assets"));
                })
                .AddSingleton<ILoggerFactory>(b => new NullLoggerFactory())
                .AddSingleton<IFileProvider, VirtualFileProvider>()
                .AddSingleton<IDynamicFileProvider, DynamicFileProvider>()
                .BuildServiceProvider();
        }
    }
}
