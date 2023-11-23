using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ArchiveFlow.FileProcessor
{
    internal class SingleEntryProcessor
    {
        private int nestLevel;
        private FileSourceType sourceType;
        private List<string> includedExtensions;
        private FileInformationFilter? includeFile;
        private FileInformationFilter? includeZipFile;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private FileExceptionHandler? handleFileException;

        public SingleEntryProcessor(int nestLevel, FileSourceType sourceType, List<string> includedExtensions, FileInformationFilter? includeFile, FileInformationFilter? includeZipFile, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, FileExceptionHandler? handleFileException)
        {
            this.nestLevel = nestLevel;
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
            HandleZip,
            HandleNonZip,
        }


        public void ProcessEntry(FileInformation file, Stream openStream)
        {

            HandleType handleType = GetHandleType(file);

            try
            {
                switch (handleType)
                {
                    case HandleType.HandleZip:
                        HandleZipFile(file, openStream);
                        break;
                    case HandleType.HandleNonZip:
                        HandleRegularFile(file, openStream);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                if (handleFileException != null)
                {
                    handleFileException(file, ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private HandleType GetHandleType(FileInformation file)
        {
            var handleType = HandleType.Nothing;

            bool processAsZipFile = sourceType.IncludesZipped() && file.Extension.IsZipExtension();

            if (processAsZipFile &&
                (this.includeZipFile == null || this.includeZipFile(file)))
            {
                handleType = HandleType.HandleZip;
            }

            if (!processAsZipFile &&
                (sourceType.IncludesUnzipped() || nestLevel > 0) &&
                includedExtensions.ContainsExtension(file.Extension) &&
                (this.includeFile == null || this.includeFile(file)))
            {
                handleType = HandleType.HandleNonZip;
            }

            return handleType;
        }


        private void HandleRegularFile(FileInformation file, Stream openStream)
        {
            throw new NotImplementedException();
        }

        private void HandleZipFile(FileInformation file, Stream openStream)
        {
            throw new NotImplementedException();
        }
    }
}
