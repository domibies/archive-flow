
using ArchiveFlow.Models;
using System.IO;
using System.Threading.Tasks;

namespace ArchiveFlow.Abstractions
{
    internal interface IStreamProcessor
    {
        void ProcessStream(FileInformation file, Stream stream);
        void MaybeProcessStream(FileInformation file, Stream stream);
        void MaybeProcessText(FileInformation file, Stream stream);
        void MaybeProcessBytes(FileInformation file, Stream stream);
        
        // Async versions
        Task ProcessStreamAsync(FileInformation file, Stream stream);
        Task MaybeProcessStreamAsync(FileInformation file, Stream stream);
        Task MaybeProcessTextAsync(FileInformation file, Stream stream);
        Task MaybeProcessBytesAsync(FileInformation file, Stream stream);
    }
}
