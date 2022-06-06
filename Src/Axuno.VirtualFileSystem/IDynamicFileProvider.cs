using Microsoft.Extensions.FileProviders;

namespace Axuno.VirtualFileSystem;

public interface IDynamicFileProvider : IFileProvider
{
    void AddOrUpdate(IFileInfo fileInfo);

    bool Delete(string filePath);
}
