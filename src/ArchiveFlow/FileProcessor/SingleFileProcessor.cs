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
        private int nestLevel = 0;
        private FileSourceType sourceType;
        private List<string> includedExtensions;
        private FileInformationFilter? includeFile;
        private FileInformationFilter? includeZipFile;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private FileExceptionHandler? handleFileException;

        public SingleFileProcessor(FileSourceType sourceType, List<string> includedExtensions, FileInformationFilter? includeFile, FileInformationFilter? includeZipFile, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, FileExceptionHandler? handleFileException)
        {
            this.sourceType = sourceType;
            this.includedExtensions = includedExtensions;
            this.includeFile = includeFile;
            this.includeZipFile = includeZipFile;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.handleFileException = handleFileException;
        }

        internal enum HandleType
        {
            Nothing,
            HandleZipFile,
            HandleRegularFile,
        }

        public void ProcessFile(FileInfo file)
        {
            var fileInfo = file.ToFileInformation();

            bool processAsZipFile = sourceType.IncludesZipped() && file.Extension.IsZipExtension();

            var handleType = HandleType.Nothing;

            if (processAsZipFile && (this.includeZipFile == null || this.includeZipFile(fileInfo)))
                handleType = HandleType.HandleZipFile;

            if (!processAsZipFile && includedExtensions.ContainsExtension(file.Extension) && (this.includeFile == null || this.includeFile(fileInfo)))
                handleType = HandleType.HandleRegularFile;

            try
            {
                switch (handleType)
                {
                    case HandleType.HandleZipFile:
                        IZipFileProcessor zipFileProcessor = new SharpCompressZipFileProcessor(includedExtensions, includeFile, streamProcessingAction, textProcessingAction, bytesProcessingAction, handleFileException);
                        zipFileProcessor.ProcessZipFile(file);
                        break;
                    case HandleType.HandleRegularFile:
                        using (Stream stream = file.OpenRead())
                        {
                            var streamProcessor = new StreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction);
                            streamProcessor.ProcessStream(stream);
                        }
                        break;
                    default:
                        // Do nothing
                        break;
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
