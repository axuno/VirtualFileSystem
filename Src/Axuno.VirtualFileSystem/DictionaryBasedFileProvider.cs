using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;

namespace Axuno.VirtualFileSystem;

public abstract class DictionaryBasedFileProvider : IFileProvider
{
    protected abstract IDictionary<string, IFileInfo> Files { get; }

    public virtual IFileInfo GetFileInfo(string subPath)
    {
        if (subPath == null)
        {
            return new NotFoundFileInfo(subPath ?? "(null)");
        }

        var file = Files.GetOrDefault(NormalizePath(subPath));

        if (file == null)
        {
            return new NotFoundFileInfo(subPath);
        }

        return file;
    }

    public virtual IDirectoryContents GetDirectoryContents(string subPath)
    {
        var directory = GetFileInfo(subPath);
        if (!directory.IsDirectory)
        {
            return NotFoundDirectoryContents.Singleton;
        }

        var fileList = new List<IFileInfo>();

        var directoryPath = subPath.EnsureEndsWith('/');
        foreach (var fileInfo in Files.Values)
        {
            var fullPath = fileInfo.GetVirtualOrPhysicalPath();
            if (fullPath is null || !fullPath.StartsWith(directoryPath))
            {
                continue;
            }

            var relativePath = fullPath[directoryPath.Length..];
            if (relativePath.Contains('/'))
            {
                continue;
            }

            fileList.Add(fileInfo);
        }

        return new EnumerableDirectoryContents(fileList);
    }

    public virtual IChangeToken Watch(string filter)
    {
        return NullChangeToken.Singleton;
    }

    protected virtual string NormalizePath(string subPath)
    {
        return subPath;
    }
}
