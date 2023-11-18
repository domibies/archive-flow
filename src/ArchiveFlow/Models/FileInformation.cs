using System;
using ArchiveFlow.Abstractions;
using SharpCompress.Archives;

namespace ArchiveFlow.Models
{
    internal class FileInformation : IFileInformation
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsCompressed { get; set; }
        public IArchive? ArchiveReference { get; set; } // only for internal

        public FileInformation(string fileName, string extension, long size, DateTime lastModified, bool isCompressed, IArchive? archiveReference = null)
        {
            FileName = fileName;
            Extension = extension;
            Size = size;
            LastModified = lastModified;
            IsCompressed = isCompressed;
            ArchiveReference = archiveReference;
        }
    }
}
