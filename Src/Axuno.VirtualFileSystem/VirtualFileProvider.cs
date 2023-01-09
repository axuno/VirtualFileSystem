using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Axuno.VirtualFileSystem;

/// <summary>
/// Virtual file provider
/// </summary>
/// <remarks>
/// Inject as singleton
/// </remarks>
public class VirtualFileProvider : IFileProvider
{
    private readonly IFileProvider _compositeFileProvider;
    private readonly VirtualFileSystemOptions _options;

    public VirtualFileProvider(
        IOptions<VirtualFileSystemOptions> options,
        IDynamicFileProvider dynamicFileProvider)
    {
        _options = options.Value;
        _compositeFileProvider = CreateCompositeProviderPrivate(dynamicFileProvider);
    }

    public virtual IFileInfo GetFileInfo(string subPath)
    {
        var fileInfo = _compositeFileProvider.GetFileInfo(subPath);
        if (fileInfo.Exists) return fileInfo;
            
        // This is a workaround for the PhysicalFileProvider, as it does not return an IFileInfo for existing directories.
        // The EmbeddedFileProvider WILL return the IFileInfo for a directory.
        // So the path is not a file, but it could be a directory:
        var pathSegments = subPath.Split('/');
        var content = _compositeFileProvider.GetDirectoryContents(string.Join('/', pathSegments.SkipLast(1)));
        var directory = content.FirstOrDefault(fi => fi.Name == pathSegments[^1] && fi.IsDirectory);

        return directory is { } ? new VirtualDirectoryFileInfo(directory.PhysicalPath ?? string.Empty, directory.Name, directory.LastModified) : fileInfo;
    }

    public virtual IDirectoryContents GetDirectoryContents(string subPath)
    {
        if (subPath == "")
        {
            subPath = "/";
        }
            
        return _compositeFileProvider.GetDirectoryContents(subPath);
    }

    public virtual IChangeToken Watch(string filter)
    {
        return _compositeFileProvider.Watch(filter);
    }

    protected virtual IFileProvider CreateCompositeProvider(IDynamicFileProvider dynamicFileProvider)
    {
        return CreateCompositeProviderPrivate(dynamicFileProvider);
    }

    private IFileProvider CreateCompositeProviderPrivate(IFileProvider dynamicFileProvider)
    {
        var fileProviders = new List<IFileProvider> {dynamicFileProvider};
            
        foreach (var fileSet in _options.FileSets.AsEnumerable().Reverse())
        {
            fileProviders.Add(fileSet.FileProvider);
        }

        return new CompositeFileProvider(fileProviders);
    }
}
