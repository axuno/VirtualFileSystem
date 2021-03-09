using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace Axuno.VirtualFileSystem.Embedded
{
    /// <summary>
    /// Represents a file embedded in an assembly.
    /// </summary>
    public class EmbeddedResourceFileInfo : IFileInfo
    {
        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="resourcePath"></param>
        /// <param name="virtualPath"></param>
        /// <param name="name"></param>
        /// <param name="lastModified"></param>
        public EmbeddedResourceFileInfo(
            Assembly assembly,
            string resourcePath,
            string virtualPath,
            string name,
            DateTimeOffset lastModified)
        {
            _assembly = assembly;
            _resourcePath = resourcePath;

            VirtualPath = virtualPath;
            Name = name;
            LastModified = lastModified;
        }
        
        public bool Exists => true;

        public long Length
        {
            get
            {
                if (!_length.HasValue)
                {
                    using var stream = _assembly.GetManifestResourceStream(_resourcePath);
                    _length = stream?.Length;
                }

                return 0;
            }
        }
        private long? _length;

        public string? PhysicalPath => null;

        public string VirtualPath { get; }

        public string Name { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/>.
        /// </summary>
        public DateTimeOffset LastModified { get; }

        public bool IsDirectory => false;

        private readonly Assembly _assembly;
        private readonly string _resourcePath;

        /// <inheritdoc />
        public Stream? CreateReadStream()
        {
            var stream = _assembly.GetManifestResourceStream(_resourcePath);

            if (!_length.HasValue && stream != null)
            {
                _length = stream.Length;
            }

            return stream;
        }

        public override string ToString()
        {
            return $"[EmbeddedResourceFileInfo] {Name} ({this.VirtualPath})";
        }
    }
}