
using ArchiveFlow.Models;
using System.IO;

namespace ArchiveFlow.Abstractions
{
    internal interface IStreamProcessor
    {
        void ProcessStream(FileInformation file, Stream stream);
        void MaybeProcessStream(FileInformation file, Stream stream);
        void MaybeProcessText(FileInformation file, Stream stream);
        void MaybeProcessBytes(FileInformation file, Stream stream);
    }
}
