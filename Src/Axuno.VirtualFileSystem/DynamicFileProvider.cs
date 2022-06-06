using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Axuno.VirtualFileSystem;

/// <summary>
/// Dynamic file provider
/// </summary>
/// <remarks>
/// Inject as singleton
/// Current implementation only supports file watch.
/// Does not support directory or wildcard watches.
/// </remarks>
public class DynamicFileProvider : DictionaryBasedFileProvider, IDynamicFileProvider
{
    /// <summary>
    /// Gets the <see cref="IDictionary{TKey,TValue}"/> for dynamic files.
    /// </summary>
    /// <remarks>This property can be overridden in a derived class.</remarks>
    protected override IDictionary<string, IFileInfo> Files => DynamicFiles;

    /// <summary>
    /// Gets dynamic files as a <see cref="ConcurrentDictionary{TKey,TValue}"/>.
    /// </summary>
    protected ConcurrentDictionary<string, IFileInfo> DynamicFiles { get; }

    /// <summary>
    /// Gets the <see cref="ConcurrentDictionary{TKey,TValue}"/> for <see cref="ChangeTokenInfo"/>s.
    /// </summary>
    protected ConcurrentDictionary<string, ChangeTokenInfo> FilePathTokenLookup { get; }

    /// <summary>
    /// CTOR.
    /// </summary>
    public DynamicFileProvider()
    {
        FilePathTokenLookup = new ConcurrentDictionary<string, ChangeTokenInfo>(StringComparer.OrdinalIgnoreCase);;
        DynamicFiles = new ConcurrentDictionary<string, IFileInfo>();
    }

    /// <summary>
    /// Updates the existing <see cref="IFileInfo"/> or inserts a new one.
    /// </summary>
    /// <param name="fileInfo"></param>
    public void AddOrUpdate(IFileInfo fileInfo)
    {
        var filePath = fileInfo.GetVirtualOrPhysicalPath();
        if (filePath is null) return;
        DynamicFiles.AddOrUpdate(filePath, fileInfo, (key, value) => fileInfo);
        ReportChange(filePath);
    }

    /// <summary>
    /// Deletes a dynamic file.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns><see langword="true"/>, if the dynamic file was deleted.</returns>
    public bool Delete(string filePath)
    {
        if (!DynamicFiles.TryRemove(filePath, out _))
        {
            return false;
        }

        ReportChange(filePath);
        return true;
    }

    /// <summary>
    /// Gets the <see cref="IChangeToken"/> for the given filter.
    /// </summary>
    /// <param name="filter"></param>
    /// <returns>The <see cref="IChangeToken"/> for the given filter.</returns>
    public override IChangeToken Watch(string filter)
    {
        return GetOrAddChangeToken(filter);
    }

    private IChangeToken GetOrAddChangeToken(string filePath)
    {
        if (!FilePathTokenLookup.TryGetValue(filePath, out var tokenInfo))
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationChangeToken = new CancellationChangeToken(cancellationTokenSource.Token);
            tokenInfo = new ChangeTokenInfo(cancellationTokenSource, cancellationChangeToken);
            tokenInfo = FilePathTokenLookup.GetOrAdd(filePath, tokenInfo);
        }

        return tokenInfo.ChangeToken;
    }

    private void ReportChange(string filePath)
    {
        if (FilePathTokenLookup.TryRemove(filePath, out var tokenInfo))
        {
            tokenInfo.TokenSource.Cancel();
        }
    }

    /// <summary>
    /// The <see cref="ChangeTokenInfo"/> structure.
    /// </summary>
    protected struct ChangeTokenInfo
    {
        /// <summary>
        /// CTOR.
        /// </summary>
        /// <param name="tokenSource"></param>
        /// <param name="changeToken"></param>
        public ChangeTokenInfo(
            CancellationTokenSource tokenSource,
            CancellationChangeToken changeToken)
        {
            TokenSource = tokenSource;
            ChangeToken = changeToken;
        }

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/>.
        /// </summary>
        public CancellationTokenSource TokenSource { get; }

        /// <summary>
        /// Gets the <see cref="ChangeToken"/>.
        /// </summary>
        public CancellationChangeToken ChangeToken { get; }
    }
}
