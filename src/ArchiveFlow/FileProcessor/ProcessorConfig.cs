using ArchiveFlow.Common;
using ArchiveFlow.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ArchiveFlow.FileProcessor
{
    internal class ProcessorConfig
    {
        public string FolderPath { get; private set; }
        public FolderSelect FolderSelect { get; private set; }
        public ArchiveSearch ArchiveSearch { get; private set; }
        public IEnumerable<string> IncludedExtensions { get; private set; }
        public FileInformationFilter? FileFilter { get; private set; }
        public FileInformationFilter? ZipFileFilter { get; private set; }
        public StreamProcessingAction? StreamProcessingAction { get; private set; }
        public TextProcessingAction? TextProcessingAction { get; private set; }
        public BytesProcessingAction? BytesProcessingAction { get; private set; }
        public FileExceptionHandler? HandleFileException { get; private set; }
        public int? MaxDegreeOfParallelism { get; private set; }

        public ProcessorConfig(
            string folderPath = "",
            FolderSelect folderSelect = default,
            ArchiveSearch archiveSearch = default,
            IEnumerable<string>? includedExtensions = null,
            FileInformationFilter? fileFilter = null,
            FileInformationFilter? zipFileFilter = null,
            StreamProcessingAction? streamProcessingAction = null,
            TextProcessingAction? textProcessingAction = null,
            BytesProcessingAction? bytesProcessingAction = null,
            FileExceptionHandler? handleFileException = null,
            int? maxDegreeOfParallelism = null)
        {
            FolderPath = folderPath;
            FolderSelect = folderSelect;
            ArchiveSearch = archiveSearch;
            IncludedExtensions = includedExtensions ?? new List<string>();
            FileFilter = fileFilter;
            ZipFileFilter = zipFileFilter;
            StreamProcessingAction = streamProcessingAction;
            TextProcessingAction = textProcessingAction;
            BytesProcessingAction = bytesProcessingAction;
            HandleFileException = handleFileException;
            MaxDegreeOfParallelism = maxDegreeOfParallelism;
        }
    }
}
