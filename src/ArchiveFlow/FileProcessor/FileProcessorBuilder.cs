using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ArchiveFlow.FileProcessor
{
    public class FileProcessorBuilder
    {
        private string? folderPath;
        private FolderSelect folderSelect = default;
        private ArchiveSearch archiveSearch = default;
        private List<string> extensions = new List<string>() { };
        private FileInformationFilter? fileFilter;
        private FileInformationFilter? zipFileFilter;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private FileExceptionHandler? handleFileException;
        private int? maxDegreeOfParallelism;

        public FileProcessorBuilder FromFolder(string path, FolderSelect folderSelect = FolderSelect.RootFolderOny)
        {
            Guard.AgainstNull(nameof(path), path);

            this.folderPath = path;
            this.folderSelect = folderSelect;
            return this;
        }

        public FileProcessorBuilder SetArchiveSearch(ArchiveSearch type)
        {
            archiveSearch = type;
            return this;
        }

        public FileProcessorBuilder WithExtension(params string[] exts)
        {
            Guard.AgainstNull(nameof(exts), exts);
            Guard.AgainstSmallerThan(nameof(exts.Length), exts.Length, 1);

            extensions.AddRange(exts);
            return this;
        }

        public FileProcessorBuilder WhereFile(FileInformationFilter filter)
        {
            Guard.AgainstNull(nameof(filter), filter);

            fileFilter = filter;
            return this;
        }

        public FileProcessorBuilder FromZipWhere(FileInformationFilter filter)
        {
            Guard.AgainstNull(nameof(filter), filter);

            zipFileFilter = filter;
            return this;
        }

        public FileProcessorBuilder ProcessAsStream(StreamProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            streamProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessAsText(TextProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            textProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessAsBytes(BytesProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            bytesProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder HandleExceptionWith(FileExceptionHandler handler)
        {
            Guard.AgainstNull(nameof(handler), handler);

            handleFileException = handler;
            return this;
        }

        public FileProcessorBuilder WithMaxDegreeOfParallelism(int maxDegree)
        {
            Guard.AgainstSmallerThan(nameof(maxDegree), maxDegree, 1);

            maxDegreeOfParallelism = maxDegree;
            return this;
        }

        private ProcessorConfig GetConfig()
        {
            return new ProcessorConfig(
                               folderPath: folderPath ?? "",
                               folderSelect: folderSelect,
                               archiveSearch: archiveSearch,
                               includedExtensions: extensions,
                               fileFilter: fileFilter,
                               zipFileFilter: zipFileFilter,
                               streamProcessingAction: streamProcessingAction,
                               textProcessingAction: textProcessingAction,
                               bytesProcessingAction: bytesProcessingAction,
                               handleFileException: handleFileException,
                               maxDegreeOfParallelism: maxDegreeOfParallelism);
        }

        public FileProcessor Build()
        {
            if (folderPath == null)
                throw new InvalidOperationException("Cannot build, no FromFolder() defined.");

            if (streamProcessingAction == null && textProcessingAction == null && bytesProcessingAction == null)
                throw new InvalidOperationException("Cannot build, no ProcessAsXXX() defined.");

            int actionCount = 0;
            actionCount += streamProcessingAction != null ? 1 : 0;
            actionCount += textProcessingAction != null ? 1 : 0;
            actionCount += bytesProcessingAction != null ? 1 : 0;

            if (actionCount > 1)
            {
                throw new InvalidOperationException("Cannot build, more than one ProcessAsXXX() defined.");
            }

            return new FileProcessor(GetConfig());
        }
    }
}

