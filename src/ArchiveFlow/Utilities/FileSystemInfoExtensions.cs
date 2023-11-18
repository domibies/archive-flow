using ArchiveFlow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArchiveFlow.Utilities
{
    internal static class FileSystemInfoExtensions
    {
        public static FileInformation ToFileInformation(this FileSystemInfo fileSystemInfo)
        {
            Guard.AgainstNull(nameof(fileSystemInfo), fileSystemInfo);

            return new FileInformation(
                fileSystemInfo.Name, 
                fileSystemInfo.Extension, 
                fileSystemInfo is FileInfo ? ((FileInfo)fileSystemInfo).Length : 0, 
                fileSystemInfo.LastWriteTime, 
                false);
        }
    }
}
