﻿using System;
using System.Linq;

namespace Axuno.VirtualFileSystem;

internal static class VirtualFilePathHelper
{
    public static string NormalizePath(string fullPath)
    {
        if (fullPath.Equals("/", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        var fileName = fullPath;
        var extension = "";

        if (fileName.Contains('.'))
        {
            extension = fullPath[fileName.LastIndexOf(".", StringComparison.Ordinal)..];
            if (extension.Contains('/'))
            {
                //That means the file does not have extension, but a directory has "." char. So, clear extension.
                extension = "";
            }
            else
            {
                fileName = fullPath[..^extension.Length];
            }
        }

        return NormalizeChars(fileName) + extension;
    }

    private static string NormalizeChars(string fileName)
    {
        var folderParts = fileName.Replace(".", "/").Split("/");

        if (folderParts.Length == 1)
        {
            return folderParts[0];
        }

        return string.Join("/",folderParts.Take(folderParts.Length - 1).Select(s => s.Replace("-", "_"))) + "/" + folderParts.Last();
    }
}
