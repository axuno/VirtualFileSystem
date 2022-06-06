using Microsoft.Extensions.FileProviders;

namespace Axuno.VirtualFileSystem;

public class VirtualFileSetInfo
{
    public IFileProvider FileProvider { get; }

    public VirtualFileSetInfo(IFileProvider fileProvider)
    {
        FileProvider = Check.NotNull(fileProvider, nameof(fileProvider));
    }
}
