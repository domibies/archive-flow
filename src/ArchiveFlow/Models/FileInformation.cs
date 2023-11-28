using System;
using ArchiveFlow.Abstractions;
using SharpCompress.Archives;

namespace ArchiveFlow.Models
{
    internal class FileInformation : IFileInformation
    {
        // the path of the parent folder or the name of the zip archive
        public string Volume { get; set; }
        // the filename without the path, including the extension
        public string FileName { get; set; }
        // the extension, including the dot
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool InArchive { get; set; }
        //public IArchive? ArchiveReference { get; set; } // only for internal

        public FileInformation(string volume, string fileName, string extension, long size, DateTime lastModified, bool inArchive/*, IArchive? archiveReference = null*/)
        {
            Volume = volume;
            FileName = fileName;
            Extension = extension;
            Size = size;
            LastModified = lastModified;
            InArchive = inArchive;
            //ArchiveReference = archiveReference;
        }
    }
}
