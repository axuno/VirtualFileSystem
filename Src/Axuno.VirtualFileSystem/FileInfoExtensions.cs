using System.IO;
using System.Text;
using System.Threading.Tasks;
using Axuno.VirtualFileSystem;
using Axuno.VirtualFileSystem.Embedded;

namespace Microsoft.Extensions.FileProviders
{
    public static class FileInfoExtensions
    {
        /// <summary>
        /// Reads file content as string using <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        public static string ReadAsString( this IFileInfo fileInfo)
        {
            return fileInfo.ReadAsString(Encoding.UTF8);
        }

        /// <summary>
        /// Reads file content as string using <see cref="Encoding.UTF8"/> encoding.
        /// </summary>
        public static Task<string> ReadAsStringAsync( this IFileInfo fileInfo)
        {
            return fileInfo.ReadAsStringAsync(Encoding.UTF8);
        }

        /// <summary>
        /// Reads file content as string using the given <paramref name="encoding"/>.
        /// </summary>
        public static string ReadAsString( this IFileInfo fileInfo, Encoding encoding)
        {
            Check.NotNull(fileInfo, nameof(fileInfo));

            using var stream = fileInfo.CreateReadStream();
            using var streamReader = new StreamReader(stream, encoding, true);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Reads file content as string using the given <paramref name="encoding"/>.
        /// </summary>
        public static async Task<string> ReadAsStringAsync( this IFileInfo fileInfo, Encoding encoding)
        {
            Check.NotNull(fileInfo, nameof(fileInfo));

            await using var stream = fileInfo.CreateReadStream();
            using var streamReader = new StreamReader(stream, encoding, true);
            return await streamReader.ReadToEndAsync();
        }

        /// <summary>
        /// Reads file content as byte[].
        /// </summary>
        public static byte[] ReadBytes( this IFileInfo fileInfo)
        {
            Check.NotNull(fileInfo, nameof(fileInfo));

            using var stream = fileInfo.CreateReadStream();
            return stream.GetAllBytes();
        }

        /// <summary>
        /// Reads file content as byte[].
        /// </summary>
        public static async Task<byte[]> ReadBytesAsync( this IFileInfo fileInfo)
        {
            Check.NotNull(fileInfo, nameof(fileInfo));

            await using var stream = fileInfo.CreateReadStream();
            return await stream.GetAllBytesAsync();
        }

        public static string? GetVirtualOrPhysicalPath( this IFileInfo fileInfo)
        {
            Check.NotNull(fileInfo, nameof(fileInfo));

            if (fileInfo is EmbeddedResourceFileInfo embeddedFileInfo)
            {
                return embeddedFileInfo.VirtualPath;
            }

            if (fileInfo is InMemoryFileInfo inMemoryFileInfo)
            {
                return inMemoryFileInfo.DynamicPath;
            }

            return fileInfo.PhysicalPath;
        }
    }
}