using ArchiveFlow.Abstractions;
using ArchiveFlow.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArchiveFlow.Common
{
    public delegate bool FileInformationFilter(IFileInformation fileInfo);
    public delegate void StreamProcessingAction(IFileInformation f, Stream stream);
    public delegate void TextProcessingAction(IFileInformation f, string text);
    public delegate void BytesProcessingAction(IFileInformation f,byte[] data);
    public delegate bool FileExceptionHandler(IFileInformation f, Exception ex);
    
    // Async versions of the processing delegates
    public delegate Task AsyncStreamProcessingAction(IFileInformation f, Stream stream, System.Threading.CancellationToken cancellationToken);
    public delegate Task AsyncTextProcessingAction(IFileInformation f, string text, System.Threading.CancellationToken cancellationToken);
    public delegate Task AsyncBytesProcessingAction(IFileInformation f, byte[] data, System.Threading.CancellationToken cancellationToken);
}
