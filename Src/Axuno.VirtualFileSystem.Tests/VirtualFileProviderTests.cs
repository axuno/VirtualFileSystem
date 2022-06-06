using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace Axuno.VirtualFileSystem.Tests;

[TestFixture]
public class VirtualFileProviderTests
{
    private readonly IFileProvider _virtualFileProvider;

    public VirtualFileProviderTests()
    {
        _virtualFileProvider =
            ServiceSetup.GetVirtualFileSystemServiceProvider().GetRequiredService<IFileProvider>();
    }

    [Test]
    public void Should_Define_And_Get_Embedded_Resources()
    {
        var resource = _virtualFileProvider.GetFileInfo("/Text/Testfile_1.txt");

        Assert.IsNotNull(resource);
        Assert.IsTrue(resource.Exists);

        // file is expected as UTF8 without BOM
        using var stream = resource.CreateReadStream();
        Assert.AreEqual("Testfile_1.txt Content No <Cr><Lf>!", Encoding.UTF8.GetString(stream.GetAllBytes()));
    }

    [Test]
    public void Should_Define_And_Get_Embedded_Resources_With_Special_Chars()
    {
        var resource = _virtualFileProvider.GetFileInfo("/Text/Testfile_{2}.txt");
        Assert.IsNotNull(resource);
        Assert.IsTrue(resource.Exists);

        // file is expected as UTF8 without BOM
        using var stream = resource.CreateReadStream();
        Assert.AreEqual("Testfile_{2}.txt Content No <Cr><Lf>!", Encoding.UTF8.GetString(stream.GetAllBytes()));
    }

    [Test]
    public void Should_Define_And_Get_Embedded_Directory_Contents()
    {
        var contents = _virtualFileProvider.GetDirectoryContents("/Text");

        Assert.IsTrue(contents.Exists);

        var contentList = contents.ToList();

        Assert.Contains("Testfile_1.txt", contentList.Select(c => c.Name).ToList());
        Assert.Contains("Testfile_{2}.txt", contentList.Select(c => c.Name).ToList());
    }

    [TestCase("/")]
    [TestCase("")]
    public void Should_Define_And_Get_Embedded_Root_Directory_Contents(string path)
    {
        var contents = _virtualFileProvider.GetDirectoryContents(path);

        Assert.IsTrue(contents.Exists);

        var contentList = contents.ToList();
        Assert.Contains("Text", contents.ToList().Select(c => c.Name).ToList());
    }
}
