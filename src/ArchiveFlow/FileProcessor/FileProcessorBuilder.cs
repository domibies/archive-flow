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
        private string folderPath = ".";
        private FileReadMode readMode = FileReadMode.Text;
        private FileSourceType? sourceType = FileSourceType.Both;
        private List<string> extensions = new List<string>();
        private FileInformationFilter? fileFilter;
        private FileInformationFilter? zipFileFilter;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private int? maxDegreeOfParallelism;

        public FileProcessorBuilder FromFolder(string path)
        {
            Guard.AgainstNull(nameof(path), path);

            folderPath = path;
            return this;
        }

        public FileProcessorBuilder ReadAsText()
        {
            readMode = FileReadMode.Text;
            return this;
        }

        public FileProcessorBuilder ReadAsBinary()
        {
            readMode = FileReadMode.Binary;
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

            streamProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessTextWith(TextProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            textProcessingAction = action;
            return this;
        }

        public FileProcessorBuilder ProcessBytesWith(BytesProcessingAction action)
        {
            Guard.AgainstNull(nameof(action), action);

            bytesProcessingAction = action;
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
            return new FileProcessor(folderPath, readMode, sourceType, extensions, fileFilter, zipFileFilter, streamProcessingAction, textProcessingAction, bytesProcessingAction, maxDegreeOfParallelism);
        }
    }
}

