using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static System.Collections.Specialized.BitVector32;

namespace ArchiveFlow.FileProcessor
{
    public class FileProcessorBuilder
    {
        private string? folderPath;
        private RecurseOption recurseOption;
        private FileSourceType? sourceType;
        private List<string> extensions = new List<string>() { };
        private FileInformationFilter? fileFilter;
        private FileInformationFilter? zipFileFilter;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private int? maxDegreeOfParallelism;
        private FileExceptionHandler? handleFileException;

        public FileProcessorBuilder FromFolder(string path, RecurseOption recurse = RecurseOption.RecurseNo)
        {
            Guard.AgainstNull(nameof(path), path);

            folderPath = path;
            recurseOption = recurse;
            return this;
        }

        public FileProcessorBuilder UseSource(FileSourceType type)
        {
            sourceType = type;
            return this;
        }

        public FileProcessorBuilder FilterByExtension(params string[] exts)
        {
            Guard.AgainstNull(nameof(exts), exts);
            Guard.AgainstSmallerThan(nameof(exts.Length), exts.Length, 1);

            extensions.AddRange(exts);
            return this;
        }

        public FileProcessorBuilder Where(FileInformationFilter filter)
        {
            Guard.AgainstNull(nameof(filter), filter);

            fileFilter = filter;
            return this;
        }

        public FileProcessorBuilder WhereZip(FileInformationFilter filter)
        {
            Guard.AgainstNull(nameof(filter), filter);

            zipFileFilter = filter;
            return this;
        }

        public FileProcessorBuilder ProcessStreamWith(StreamProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            if (textProcessingAction != null || bytesProcessingAction != null)
                throw new InvalidOperationException("Cannot process, only one ProcessXXXWith() allowed.");

            streamProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessTextWith(TextProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            if (streamProcessingAction != null || bytesProcessingAction != null)
                throw new InvalidOperationException("Cannot process, only one ProcessXXXWith() allowed.");

            textProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessBytesWith(BytesProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            if (streamProcessingAction != null || textProcessingAction != null)
                throw new InvalidOperationException("Cannot process, only one ProcessXXXWith() allowed.");

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

        public FileProcessor Build()
        {
            if (folderPath == null)
                throw new InvalidOperationException("Cannot build, no FromFolder() defined.");

            if (streamProcessingAction == null && textProcessingAction == null && bytesProcessingAction == null)
                throw new InvalidOperationException("Cannot build, no ProcessXXXWith() defined.");

            if (sourceType == null)
                throw new InvalidOperationException("Cannot build, no UseSource() defined.");

            return new FileProcessor(folderPath, recurseOption, (FileSourceType)sourceType, extensions, fileFilter, zipFileFilter, streamProcessingAction, textProcessingAction, bytesProcessingAction, maxDegreeOfParallelism, handleFileException);
        }
    }
}

