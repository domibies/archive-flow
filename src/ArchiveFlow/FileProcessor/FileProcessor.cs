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

namespace ArchiveFlow.FileProcessor
{    
    public class FileProcessor
    {
        private string folderPath;
        private FileSourceType? sourceType = FileSourceType.ZippedAndUnzipped;
        private List<string> includedExtensions = new List<string>();
        private FileInformationFilter? includeFile;
        private FileInformationFilter? includeZipFile;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private int? maxDegreeOfParallelism;

        internal FileProcessor(string folderPath, FileSourceType? sourceType, List<string> extensions, FileInformationFilter? fileFilter, FileInformationFilter? zipFileFilter, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, int? maxDegreeOfParallelism)
        {
            this.folderPath = folderPath;
            this.sourceType = sourceType;
            this.includedExtensions = extensions;
            this.includeFile = fileFilter;
            this.includeZipFile = zipFileFilter;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public void ProcessFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            foreach ((var file, var fileInfo) in directory.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => includedExtensions.Contains(f.Extension)).Select(f => (f, f.ToFileInformation())))
            {
                if (includeFile == null || includeFile(file.ToFileInformation()))
                {
                    using (Stream stream = file.OpenRead())
                    {
                        var streamProcessor = new StreamProcessor(streamProcessingAction, textProcessingAction, bytesProcessingAction); 
                        streamProcessor.ProcessStream(stream);
                    }


                    if (file.Extension.IsZipExtension() && (includeZipFile == null || includeZipFile(fileInfo)))
                    {
                        IZipFileProcessor zipFileProcessor = new SharpCompressZipFileProcessor(includedExtensions, includeFile, streamProcessingAction, textProcessingAction, bytesProcessingAction);
                        zipFileProcessor.ProcessZipFile(file);  
                    }
                }
            }
            foreach ((var file, var fileInfo) in directory.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => includedExtensions.Contains(f.Extension)).Select(f => (f, f.ToFileInformation())))
            {
                // Code to be executed for each file and fileInfo
            }
        }
    }
}
