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
using System.Threading;
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
                    (file) =>
                    {
                        using (var stream = file.OpenRead())
                        {
                            new SingleEntryProcessor(config).ProcessEntry(file.ToFileInformation(), stream);
                        }
                    });
            }
            else
            {
                foreach (FileInfo file in directory.EnumerateFiles("*", config.FolderSelect == FolderSelect.RootAndSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly))
                {
                    using (var stream = file.OpenRead())
                    {
                        new SingleEntryProcessor(config).ProcessEntry(file.ToFileInformation(), stream);
                    }
                }
            }
        }

        public async Task ProcessFilesAsync()
        {
            DirectoryInfo directory = new DirectoryInfo(config.FolderPath);
            var files = directory.EnumerateFiles("*", config.FolderSelect == FolderSelect.RootAndSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            if (config.MaxDegreeOfParallelism != null && config.MaxDegreeOfParallelism > 1)
            {
                // Use SemaphoreSlim to control concurrency for async operations
                using (var semaphore = new System.Threading.SemaphoreSlim(config.MaxDegreeOfParallelism.Value))
                {
                    var tasks = files.Select(async file =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            using (var stream = file.OpenRead())
                            {
                                await new SingleEntryProcessor(config).ProcessEntryAsync(file.ToFileInformation(), stream);
                            }
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    });
                    
                    await Task.WhenAll(tasks);
                }
            }
            else
            {
                foreach (FileInfo file in files)
                {
                    using (var stream = file.OpenRead())
                    {
                        await new SingleEntryProcessor(config).ProcessEntryAsync(file.ToFileInformation(), stream);
                    }
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
