using ArchiveFlow.Abstractions;
using ArchiveFlow.Models;
using System;
using System.IO;

namespace ArchiveFlow.Common
{
    public delegate bool FileInformationFilter(IFileInformation fileInfo);
    public delegate void StreamProcessingAction(Stream stream);
    public delegate void TextProcessingAction(string text);
    public delegate void BytesProcessingAction(byte[] data);
    public delegate bool ExceptionHandler(IFileInformation f, Exception ex);
}
