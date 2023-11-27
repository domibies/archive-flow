using ArchiveFlow.Abstractions;
using ArchiveFlow.Models;
using System;
using System.IO;

namespace ArchiveFlow.Common
{
    public delegate bool FileInformationFilter(IFileInformation fileInfo);
    public delegate void StreamProcessingAction(IFileInformation f, Stream stream);
    public delegate void TextProcessingAction(IFileInformation f, string text);
    public delegate void BytesProcessingAction(IFileInformation f,byte[] data);
    public delegate bool FileExceptionHandler(IFileInformation f, Exception ex);
}
