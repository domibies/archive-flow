using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.IO;
using System.Linq;

namespace ArchiveFlow.FileProcessor
{
    internal class SingleEntryProcessor
    {
        private readonly ProcessorConfig config;

        public SingleEntryProcessor(ProcessorConfig config)
        {
            this.config = config;
        }

        internal enum HandleType
        {
            SkipIt,
            HandleAsAZipFile,
            HandleAsARegularFile,
        }


        public void ProcessEntry(FileInformation file, Stream openStream)
        {

            HandleType handleType = HowToHandle(file);

            try
            {
                switch (handleType)
                {
                    case HandleType.HandleAsAZipFile:
                        HandleAsAZipFile(file, openStream);
                        break;
                    case HandleType.HandleAsARegularFile:
                        HandleAsARegularFile(file, openStream);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                if (config.HandleFileException == null || !config.HandleFileException(file, ex))
                    throw;
            }
        }

        private HandleType HowToHandle(FileInformation file)
        {
            var handleType = HandleType.SkipIt;

            bool processAsZipFile = config.ArchiveSearch.IncludesZipped() && file.Extension.IsZipExtension();

            if (processAsZipFile &&
                (config.ZipFileFilter == null || config.ZipFileFilter(file)))
            {
                handleType = HandleType.HandleAsAZipFile;
            }

            bool includeExtension =
                !config.IncludedExtensions.Any() ||
                config.IncludedExtensions.Any(ext => ext.Equals(file.Extension, StringComparison.OrdinalIgnoreCase));


            if (!processAsZipFile &&
                (config.ArchiveSearch.IncludesUnzipped() || file.InArchive) &&
                includeExtension &&
                (config.FileFilter == null || config.FileFilter(file)))
            {
                handleType = HandleType.HandleAsARegularFile;
            }

            return handleType;
        }


        private void HandleAsARegularFile(FileInformation file, Stream openStream)
        {
            var streamProcessor = new StreamProcessor(config.StreamProcessingAction, config.TextProcessingAction, config.BytesProcessingAction);
            streamProcessor.ProcessStream(file, openStream);
        }

        private void HandleAsAZipFile(FileInformation file, Stream openStream)
        {
            var zipFileProcessor = new SharpCompressZipFileProcessor(config);

            if (!openStream.CanSeek)
            {
                var memoryStream = new MemoryStream();

                openStream.CopyTo(memoryStream);
                openStream.Dispose();
                openStream = memoryStream;
                openStream.Seek(0, SeekOrigin.Begin);
            }                
            
            zipFileProcessor.ProcessZipFile(file, openStream);            
        }
    }
}
