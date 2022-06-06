namespace Axuno.VirtualFileSystem;

public class VirtualFileSystemOptions
{
    public VirtualFileSetList FileSets { get; }
        
    public VirtualFileSystemOptions()
    {
        FileSets = new VirtualFileSetList();
    }
}
