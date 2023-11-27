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
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFlow.FileProcessor
{    
    public class FileProcessor
    {
        private readonly ProcessorConfig config;

        internal FileProcessor(ProcessorConfig config)
        {
            this.config = config;
        }

        public void ProcessFiles()
        {
            DirectoryInfo directory = new DirectoryInfo(config.FolderPath);

            if (config.MaxDegreeOfParallelism != null && config.MaxDegreeOfParallelism > 1)
            {
                Parallel.ForEach(
                    directory.EnumerateFiles("*", config.FolderSelect == FolderSelect.RootAndSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly),
                    new ParallelOptions { MaxDegreeOfParallelism = (int)config.MaxDegreeOfParallelism },
                    (file) => new SingleEntryProcessor(config).ProcessEntry(file.ToFileInformation(), file.OpenRead()));
            }
            else
            {
                foreach (FileInfo file in directory.EnumerateFiles("*", config.FolderSelect == FolderSelect.RootAndSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    new SingleEntryProcessor(config).ProcessEntry(file.ToFileInformation(), file.OpenRead());
                }
            }
        }

        private bool IsValidExtension(string extension)
        {
            var compareExtensions = config.IncludedExtensions.Union(
                config.ArchiveSearch.IncludesZipped() ?
                ZipExtension.List : new List<string>()).Distinct().ToList();
            return compareExtensions.ContainsExtension(extension);
        }
    }
}
