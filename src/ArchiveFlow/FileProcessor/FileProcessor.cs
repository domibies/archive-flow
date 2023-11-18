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
        private FileReadMode readMode = FileReadMode.Text;
        private FileSourceType? sourceType = FileSourceType.Both;
        private List<string> extensions = new List<string>();
        private FileInformationFilter? fileFilter;
        private FileInformationFilter? zipFileFilter;
        private StreamProcessingAction? streamProcessingAction;
        private TextProcessingAction? textProcessingAction;
        private BytesProcessingAction? bytesProcessingAction;
        private int? maxDegreeOfParallelism;

        internal FileProcessor(string folderPath, FileReadMode readMode, FileSourceType? sourceType, List<string> extensions, FileInformationFilter? fileFilter, FileInformationFilter? zipFileFilter, StreamProcessingAction? streamProcessingAction, TextProcessingAction? textProcessingAction, BytesProcessingAction? bytesProcessingAction, int? maxDegreeOfParallelism)
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
                FileInformation fileInfo = file.ToFileInformation();
                if (fileFilter == null || fileFilter(file.ToFileInformation())) // inside filter?
                {
                    using (Stream stream = file.OpenRead())
                    {
                        if (!(streamProcessingAction is null))
                        {
                            streamProcessingAction?.Invoke(stream);
                        }
                        else if (readMode == FileReadMode.Text && !(textProcessingAction is null))
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                textProcessingAction?.Invoke(reader.ReadToEnd());
                            }
                        }
                        else if (readMode == FileReadMode.Binary && !(bytesProcessingAction is null))
                        {
                            byte[] bytes = new byte[stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);
                            bytesProcessingAction?.Invoke(bytes);
                        }
                    }


                    if (file.Extension.IsZipExtension() && (zipFileFilter == null || zipFileFilter(fileInfo))) // inside filter?
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
