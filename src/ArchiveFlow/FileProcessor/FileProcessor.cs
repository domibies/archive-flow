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
        private FileReadMode readMode = FileReadMode.Text;
        private FileSourceType? sourceType = FileSourceType.Both;
        private List<string> extensions = new List<string>();
        private Func<FileInformation, bool>? fileFilter;
        private Func<FileInformation, bool>? zipFileFilter;
        private Action<Stream>? streamProcessingAction;
        private Action<string>? textProcessingAction;
        private Action<byte[]>? bytesProcessingAction;
        private int? maxDegreeOfParallelism;

        public FileProcessor(string folderPath, FileReadMode readMode, FileSourceType? sourceType, List<string> extensions, Func<FileInformation, bool>? fileFilter, Func<FileInformation, bool>? zipFileFilter, Action<Stream>? streamProcessingAction, Action<string>? textProcessingAction, Action<byte[]>? bytesProcessingAction, int? maxDegreeOfParallelism)
        {
            this.folderPath = folderPath;
            this.readMode = readMode;
            this.sourceType = sourceType;
            this.extensions = extensions;
            this.fileFilter = fileFilter;
            this.zipFileFilter = zipFileFilter;
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public void ProcessFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(folderPath);

            foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories).Where(f => extensions.Contains(f.Extension)))
            {
                if (fileFilter == null || fileFilter(file.ToFileInformation()))
                {
                    // Process file
                    Console.WriteLine("Processing file: " + file.FullName);

                    if (file.Extension.IsZipExtension())
                    {
                        ProcessZipFile(file);
                    }
                }
            }
        }

        private void ProcessZipFile(FileInfo zipFileInfo)
        {
            using (var archive = ArchiveFactory.Open(zipFileInfo.FullName))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory && extensions.Contains(Path.GetExtension(entry.Key).ToLower()) && (fileFilter == null || fileFilter(entry.ToFileInformation(zipFileInfo.LastWriteTime))))
                    {
                        // Process zip entry
                        Console.WriteLine("Processing (file in) zip entry: " + entry.Key);
                    }
                }
            }
        }
    }
}
