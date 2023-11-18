using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArchiveFlow.Abstractions
{
    public interface IArchiveInstance
    {
        IEnumerable<IFileInformation> GetEntries();
        string FileName { get; }
        string FullPath { get; }
        Stream GetStream(IFileInformation information);
    }
}
