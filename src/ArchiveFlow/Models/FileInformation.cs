using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveFlow.Models
{
    public class FileInformation
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }
        public bool IsCompressed { get; set; }

        public FileInformation(string fileName, string extension, long size, DateTime lastModified, bool isCompressed)
        {
            FileName = fileName;
            Extension = extension;
            Size = size;
            LastModified = lastModified;
            IsCompressed = isCompressed;
        }

        // Additional properties and methods, if necessary
    }
}
