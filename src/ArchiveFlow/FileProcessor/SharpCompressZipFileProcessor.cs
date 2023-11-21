using ArchiveFlow.Abstractions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ArchiveFlow.FileProcessor
{
    internal class SharpCompressZipFileProcessor : IZipFileProcessor
    {
        private readonly IEnumerable<string> extensions;
        private readonly FileInformationFilter? fileFilter;
        private readonly StreamProcessingAction? streamProcessingAction;
        private readonly TextProcessingAction? textProcessingAction;
        private readonly BytesProcessingAction? bytesProcessingAction;
        private readonly FileExceptionHandler? handleFileException;


        // Lambda to create IArchive from zip file name  
        private Func<string, IArchive> createIArchive = defaultCreateIArchive;

        private static IArchive defaultCreateIArchive(string zipFullName)
        {
            return ArchiveFactory.Open(zipFullName);
        }

        // Lambda to create a streamProcessor
        private Func<StreamProcessingAction?, TextProcessingAction?, BytesProcessingAction?, IStreamProcessor> createStreamProcessor = defaultCreateStreamProcessor;
        private static IStreamProcessor defaultCreateStreamProcessor(StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction)
        {
            return new StreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction);
        }



        internal SharpCompressZipFileProcessor(
            IEnumerable<string> extensions,
            FileInformationFilter? fileFilter,
            StreamProcessingAction? streamProcessingAction,
            TextProcessingAction? textProcessingAction,
            BytesProcessingAction? bytesProcessingAction,
            FileExceptionHandler? handleFileException = null,
            Func<string, IArchive> ? createIArchive = null,
            Func<StreamProcessingAction?, TextProcessingAction?, BytesProcessingAction?, IStreamProcessor> ? createStreamProcessor = null)
        {
            this.extensions = extensions;
            this.fileFilter = fileFilter;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.handleFileException = handleFileException;

            if (!(createIArchive is null))
                this.createIArchive = createIArchive;
            if (!(createStreamProcessor is null))
                this.createStreamProcessor = createStreamProcessor; 
        }

        public void ProcessZipFile(FileInfo zipFileInfo)
        {
            using (var archive = createIArchive(zipFileInfo.FullName))
            {
                foreach(var entry in archive.Entries)
                {
                    ProcessZipEntry(entry, zipFileInfo);
                }   
            }
        }

        void ProcessZipEntry(IArchiveEntry entry, FileInfo zipFileInfo)
        {
            var fileInfo = entry.ToFileInformation(zipFileInfo.LastWriteTime);
            if (!entry.IsDirectory && extensions.Contains(Path.GetExtension(entry.Key).ToLower()) && (fileFilter == null || fileFilter(fileInfo)))
            {
                try
                {
                    // open entry stream
                    using (Stream stream = entry.OpenEntryStream())
                    {
                        var streamProcessor = createStreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction);
                        streamProcessor.ProcessStream(stream);
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
