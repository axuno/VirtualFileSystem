using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Axuno.VirtualFileSystem.Embedded;

public class EmbeddedVirtualFileSetInfo : VirtualFileSetInfo
{
    public Assembly Assembly { get; }

    public string BaseFolder { get; }

    public EmbeddedVirtualFileSetInfo(
        IFileProvider fileProvider, 
        Assembly assembly,
        string baseFolder) 
        : base(fileProvider)
    {
        Assembly = assembly;
        BaseFolder = baseFolder;
    }
}