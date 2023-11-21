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
    public class FileProcessor
    {
        private string folderPath;
        private RecurseOption recurseOption;
        private FileSourceType? sourceType = FileSourceType.ZippedAndUnzipped;
        private List<string> includedExtensions = new List<string>();
        private FileInformationFilter? includeFile;
        private FileInformationFilter? includeZipFile;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private int? maxDegreeOfParallelism;
        private FileExceptionHandler? handleFileException;

        internal FileProcessor(string folderPath, RecurseOption recurseOption, FileSourceType? sourceType, List<string> extensions, FileInformationFilter? fileFilter, FileInformationFilter? zipFileFilter, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, int? maxDegreeOfParallelism, FileExceptionHandler? handleFileException)
        {
            this.folderPath = folderPath;
            this.recurseOption = recurseOption;
            this.sourceType = sourceType;
            this.includedExtensions = extensions;
            this.includeFile = fileFilter;
            this.includeZipFile = zipFileFilter;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
            this.handleFileException = handleFileException;
        }

        public void ProcessFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            if (maxDegreeOfParallelism != null && maxDegreeOfParallelism > 1)
            {
                Parallel.ForEach(
                    directory.EnumerateFiles("*", recurseOption == RecurseOption.RecurseYes ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Where(f => IsValidExtension(f.Extension)),
                    new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism ?? Environment.ProcessorCount },
                    (file) => GetSingleFileProcessor().ProcessFile(file));
            }
            else
            {
                foreach (FileInfo file in directory.EnumerateFiles("*", recurseOption == RecurseOption.RecurseYes ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).Where(f => IsValidExtension(f.Extension)))
                {
                    GetSingleFileProcessor().ProcessFile(file);
                }
            }
        }

        private bool IsValidExtension(string extension)
        {
            var compareExtensions = includedExtensions.Union(
                sourceType == FileSourceType.Zipped || sourceType == FileSourceType.ZippedAndUnzipped ?
                ZipExtension.List : new List<string>()).Distinct().ToList();
            return compareExtensions.ContainsExtension(extension);
        }

        private SingleFileProcessor GetSingleFileProcessor()
        {
            return new SingleFileProcessor(includedExtensions, includeFile, includeZipFile, streamProcessingAction, textProcessingAction, bytesProcessingAction, handleFileException);
        }
    }
}
