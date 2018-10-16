﻿using System;
using System.IO;
using System.Linq;

namespace Bunder
{
    internal static class PathHelper
    {
        public static string Combine(params string[] paths)
        {
            bool isFileSystem = paths?.Any(path => IsFileSystemPath(path)) ?? false;
            return Combine(fileSystem: isFileSystem, paths: paths);
        }

        public static string Combine(bool fileSystem, params string[] paths)
        {
            return CombineInternal(fileSystem, paths);
        }

        private static string CombineInternal(bool fileSystem, params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                throw new ArgumentNullException(nameof(paths));

            DetermineCharsToAdjust(fileSystem, out char slashToReplace, out char newSlash);

            return Path.Combine(paths).Replace(slashToReplace, newSlash);
        }

        private static void DetermineCharsToAdjust(bool fileSystem, out char slashToReplace, out char newSlash)
        {
            // NOTE: DirectorySeparatorChar = '\\'
            //       AltDirectorySeparatorChar  = '/'

            slashToReplace = fileSystem ? Path.AltDirectorySeparatorChar : Path.DirectorySeparatorChar;
            newSlash = fileSystem ? Path.DirectorySeparatorChar : Path.AltDirectorySeparatorChar;
        }

        private static void DetermineCharsToAdjust(string path, out char slashToReplace, out char newSlash)
        {
            DetermineCharsToAdjust(IsFileSystemPath(path), out slashToReplace, out newSlash);
        }

        private static bool IsFileSystemPath(string path)
        {
            return path?.Contains(Path.DirectorySeparatorChar.ToString()) ?? false;
        }
    }
}
