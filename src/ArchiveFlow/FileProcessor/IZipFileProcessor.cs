using ArchiveFlow.Models;
using System.IO;

interface IZipFileProcessor
{
    void ProcessZipFile(FileInformation zipFileInfo, Stream? stream = null);
}

