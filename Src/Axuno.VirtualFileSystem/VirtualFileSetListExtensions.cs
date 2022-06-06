using System.IO;
using System.Reflection;
using Axuno.VirtualFileSystem.Embedded;
using Axuno.VirtualFileSystem.Physical;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;

namespace Axuno.VirtualFileSystem;

public static class VirtualFileSetListExtensions
{
    public static void AddEmbedded<T>(
        this VirtualFileSetList list,
        string baseNamespace,
        string baseFolder = "")
    {
        Check.NotNull(list, nameof(list));

        var assembly = typeof(T).Assembly;
        var fileProvider = CreateFileProvider(
            assembly,
            baseNamespace,
            baseFolder
        );

        list.Add(new EmbeddedVirtualFileSetInfo(fileProvider, assembly, baseFolder));
    }

    public static void AddPhysical(
        this VirtualFileSetList list,
        string root,
        ExclusionFilters exclusionFilters = ExclusionFilters.Sensitive)
    {
        Check.NotNull(list, nameof(list));
        Check.NotNullOrWhiteSpace(root, nameof(root));

        var fileProvider = new PhysicalFileProvider(root, exclusionFilters);
        list.Add(new PhysicalVirtualFileSetInfo(fileProvider, root));
    }

    private static IFileProvider CreateFileProvider(
        Assembly assembly,
        string baseNamespace,
        string baseFolder)
    {
        Check.NotNull(assembly, nameof(assembly));

        var info = assembly.GetManifestResourceInfo("Microsoft.Extensions.FileProviders.Embedded.Manifest.xml");

        if (info == null)
        {
            return new Axuno.VirtualFileSystem.Embedded.EmbeddedFileProvider(assembly, baseNamespace);
        }

        return new ManifestEmbeddedFileProvider(assembly, baseFolder);
    }

    public static void ReplaceEmbeddedByPhysical<T>(
        this VirtualFileSetList fileSets,
        string physicalPath)
    {
        Check.NotNull(fileSets, nameof(fileSets));
        Check.NotNullOrWhiteSpace(physicalPath, nameof(physicalPath));

        var assembly = typeof(T).Assembly;

        for (var i = 0; i < fileSets.Count; i++)
        {
            if (fileSets[i] is EmbeddedVirtualFileSetInfo embeddedVirtualFileSet &&
                embeddedVirtualFileSet.Assembly == assembly)
            {
                var thisPath = physicalPath;

                if (!embeddedVirtualFileSet.BaseFolder.IsNullOrEmpty())
                {
                    thisPath = Path.Combine(thisPath, embeddedVirtualFileSet.BaseFolder);
                }

                fileSets[i] = new PhysicalVirtualFileSetInfo(new PhysicalFileProvider(thisPath), thisPath);
            }
        }
    }
}
