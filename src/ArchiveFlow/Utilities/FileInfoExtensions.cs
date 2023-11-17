using ArchiveFlow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArchiveFlow.Utilities
{
    public static class FileInfoExtensions
    {
        public static FileInformation ToFileInformation(this FileInfo fileInfo)
        {
            return new FileInformation(
                fileInfo.Name, 
                fileInfo.Extension, 
                fileInfo.Length, 
                fileInfo.LastWriteTime, 
                false);
        }
    }
}
