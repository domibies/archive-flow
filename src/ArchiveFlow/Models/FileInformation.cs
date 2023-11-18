using System;
using ArchiveFlow.Abstractions;

namespace ArchiveFlow.Models
{
    internal class FileInformation : IFileInformation
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsCompressed { get; set; }
        // object ? because we don't want a client to have a dependency on SharpCompress
        public object? ArchiveReference { get; set; }

        public FileInformation(string fileName, string extension, long size, DateTime lastModified, bool isCompressed, object? archiveReference = null)
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
