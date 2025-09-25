using ArchiveFlow.Models;
using System.IO;
using System.Threading.Tasks;

interface IZipFileProcessor
{
    void ProcessZipFile(FileInformation zipFileInfo, Stream? stream = null);
    Task ProcessZipFileAsync(FileInformation zipFileInfo, Stream? stream = null);
}

