using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;

namespace Axuno.VirtualFileSystem;

internal class EnumerableDirectoryContents : IDirectoryContents
{
    private readonly IEnumerable<IFileInfo> _entries;

    public EnumerableDirectoryContents(IEnumerable<IFileInfo> entries)
    {
        _entries = entries;
    }

    public bool Exists => true;

    public IEnumerator<IFileInfo> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _entries.GetEnumerator();
    }
}
