using System;

namespace ArchiveFlow.Abstractions
{
    public interface IFileInformation
    {
        string Volume { get; }
        string FileName { get; }
        string Extension { get;}
        bool InArchive { get; }
        DateTime LastModified { get; }
        long Size { get; }
    }
}