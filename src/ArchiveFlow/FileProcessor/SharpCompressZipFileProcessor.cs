using ArchiveFlow.Abstractions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using SharpCompress.Archives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ArchiveFlow.FileProcessor
{
    internal class SharpCompressZipFileProcessor : IZipFileProcessor
    {
        private readonly ProcessorConfig config;

        // Lambda to create IArchive from zip file name  
        private Func<string?, Stream?, IArchive> createIArchive = defaultCreateIArchive;

        private static IArchive defaultCreateIArchive(string? zipFullName, Stream? stream)
        {
            if (zipFullName == null && stream == null)
                throw new ArgumentException("Either zipFullName or stream must be provided");

            if (zipFullName != null && stream != null)
                throw new ArgumentException("Only one of zipFullName or stream must be provided");

            if (zipFullName != null)
                return ArchiveFactory.Open(zipFullName);
            else
                return ArchiveFactory.Open(stream!);
        }

        // Lambda to create a streamProcessor
        private Func<StreamProcessingAction?, TextProcessingAction?, BytesProcessingAction?, IStreamProcessor> createStreamProcessor = defaultCreateStreamProcessor;
        private static IStreamProcessor defaultCreateStreamProcessor(StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction)
        {
            return new StreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction);
        }



        internal SharpCompressZipFileProcessor(
            ProcessorConfig config,
            Func<string?, Stream?, IArchive> ? createIArchive = null,
            Func<StreamProcessingAction?, TextProcessingAction?, BytesProcessingAction?, IStreamProcessor> ? createStreamProcessor = null)
        {
            this.config = config;

            if (!(createIArchive is null))
                this.createIArchive = createIArchive;
            if (!(createStreamProcessor is null))
                this.createStreamProcessor = createStreamProcessor; 
        }

        public void ProcessZipFile(FileInformation zipFileInfo, Stream? stream = null)
        {
            if (stream is null && zipFileInfo.InArchive)
                throw new InvalidOperationException("A nested archive can only be opened via stream.");


            using (var archive = createIArchive(stream is null ? Path.Combine(zipFileInfo.Volume, zipFileInfo.FileName) : null, stream))
            {

                foreach(var entry in archive.Entries.Where((e) => e.IsDirectory == false))
                {
                    new SingleEntryProcessor(config)
                        .ProcessEntry(entry.ToFileInformation(zipFileInfo), entry.OpenEntryStream());
                }   
            }
        }

        void ProcessZipEntry(IArchiveEntry entry, FileInformation zipFileInfo)
        {
            var processor = new SingleEntryProcessor(config);
            processor.ProcessEntry(entry.ToFileInformation(zipFileInfo), entry.OpenEntryStream());
        }


    }
}
