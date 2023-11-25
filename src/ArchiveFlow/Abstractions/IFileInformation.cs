using System;

namespace ArchiveFlow.Abstractions
{
    public interface IFileInformation
    {
        string Extension { get; set; }
        string FileName { get; set; }
        bool InArchive { get; set; }
        DateTime LastModified { get; set; }
        long Size { get; set; }
    }
}