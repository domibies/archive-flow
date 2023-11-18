using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArchiveFlow.Abstractions
{
    internal interface IZipFileProcessor
    {
        void ProcessZipFile(FileInfo zipFileInfo);
    }
}
