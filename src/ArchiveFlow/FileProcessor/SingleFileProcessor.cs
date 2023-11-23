using ArchiveFlow.Abstractions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFlow.FileProcessor
{
    public class SingleFileProcessor
    {
        private List<string> includedExtensions;
        private FileInformationFilter? includeFile;
        private FileInformationFilter? includeZipFile;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private FileExceptionHandler? handleFileException;

        public SingleFileProcessor(List<string> includedExtensions, FileInformationFilter? includeFile, FileInformationFilter? includeZipFile, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, FileExceptionHandler? handleFileException)
        {
            this.includedExtensions = includedExtensions;
            this.includeFile = includeFile;
            this.includeZipFile = includeZipFile;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.handleFileException = handleFileException;
        }

        public void ProcessFile(FileInfo file)
        {
            var fileInfo = file.ToFileInformation();
            if (includeFile == null || includeFile(fileInfo))
            {
                try
                {
                    if (includedExtensions.ContainsExtension(file.Extension))
                    {
                        using (Stream stream = file.OpenRead()) 
                        {
                            var streamProcessor = new StreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction);
                            streamProcessor.ProcessStream(stream);
                        }
                    }

                    if (file.Extension.IsZipExtension() && (includeZipFile == null || includeZipFile(fileInfo)))
                    {
                        IZipFileProcessor zipFileProcessor = new SharpCompressZipFileProcessor(includedExtensions, includeFile, streamProcessingAction, textProcessingAction, bytesProcessingAction, handleFileException);
                        zipFileProcessor.ProcessZipFile(file);
                    }
                }
                catch (Exception ex)
                {
                    if (handleFileException == null || !handleFileException(fileInfo, ex))
                    {
                        throw;
                    }
                }
            }
        }
    }
}
