using NUnit.Framework;

namespace Axuno.VirtualFileSystem.Tests;

[TestFixture]
public class VirtualFilePathHelperTests
{
    [Test]
    public void NormalizePath()
    {
        // Hyphens should be normalized to underscores
        Assert.AreEqual("~/test_one/test_two/test-three.txt", VirtualFilePathHelper.NormalizePath("~/test-one/test-two/test-three.txt"));
    }
}
