using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using FluentAssertions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ArchiveFlow.Tests.IntegrationTests
{
    public class AsyncEndToEndTests
    {
        [Fact]
        public async Task EndToEnd_Read100x100TxtFiles_Sequential_Works_Async()
        {
            // MT safe.....
            var concurrentHashSet = new ConcurrentDictionary<int, byte>();

            //Prepare
            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/txt10000")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".txt")
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    var contentAsInt = int.Parse(t);
                    concurrentHashSet.TryAdd(contentAsInt, 0);
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            // In the dictionary we expect all the numbers from 1 to 10000
            // because we have 10000 files with 1 number each, spread over 100 zip files
            concurrentHashSet.Keys.Should().BeEquivalentTo(Enumerable.Range(1, 10000));
        }

        [Fact]
        public async Task EndToEnd_Read100x100TxtFiles_InParallel_Works_Async()
        {
            // MT safe.....
            var concurrentHashSet = new ConcurrentDictionary<int, byte>();

            //Prepare
            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/txt10000")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".txt")
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    var contentAsInt = int.Parse(t);
                    concurrentHashSet.TryAdd(contentAsInt, 0);
                })
                .WithMaxDegreeOfParallelism(Environment.ProcessorCount);

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            concurrentHashSet.Keys.Should().BeEquivalentTo(Enumerable.Range(1, 10000));
        }

        [Theory]
        [InlineData(ArchiveSearch.SearchInArchivesOnly, 3, 0)]
        [InlineData(ArchiveSearch.SearchOutsideArchivesOnly, 0, 2)]
        [InlineData(ArchiveSearch.SearchInAndOutsideArchives, 3, 2)]
        public async Task EndToEnd_ArchiveSearchInAndOutsideArchives_Works_Async(ArchiveSearch archiveSearch, int expectedCountInside, int expectedCountOutside)
        {
            // MT safe.....
            var concurrentHashSet = new ConcurrentDictionary<int, byte>();

            int countInside = 0, countOutside = 0;

            //Prepare
            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/txt_in_and_outside")
                .SetArchiveSearch(archiveSearch)
                .WithExtension(".txt")
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Delay(1); // Simulate some async work
                    if (t.Contains("inside"))
                    {
                        Interlocked.Increment(ref countInside);
                    }
                    else if (t.Contains("outside"))
                    {
                        Interlocked.Increment(ref countOutside);
                    }
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            countInside.Should().Be(expectedCountInside);
            countOutside.Should().Be(expectedCountOutside);
        }

        [Fact]
        public async Task EndToEnd_NestedArchives_Work_Async()
        {
            //Prepare
            bool foundText = false;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/nested_archives")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".txt")
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    foundText = t.Contains("just a file");
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            foundText.Should().BeTrue();
        }

        [Theory]
        [InlineData(new string[] { ".txt" }, 4)]
        [InlineData(new string[] { ".log" }, 3)]
        [InlineData(new string[] { ".txt", ".log" }, 7)]
        public async Task EndToEnd_DifferentExtensions_Work_Async(string[] extensions, int expectedCount)
        {
            int count = 0;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/different_extensions")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(extensions)
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    Interlocked.Increment(ref count);
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            count.Should().Be(expectedCount);
        }

        [Fact]
        public async Task EndToEnd_RootAndSubFolders_Works_Async()
        {
            int count = 0;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/subfolders_and_filters", FolderSelect.RootAndSubFolders)
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    Interlocked.Increment(ref count);
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            count.Should().Be(401);
        }

        [Fact]
        public async Task EndToEnd_ProcessAsStreamAsync_Works()
        {
            int count = 0;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/txt_in_and_outside")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".txt")
                .ProcessAsStreamAsync(async (f, stream) =>
                {
                    await Task.Yield(); // Simulate some async work
                    // Just verify we can read from the stream
                    using (var reader = new System.IO.StreamReader(stream))
                    {
                        var content = await reader.ReadToEndAsync();
                        if (!string.IsNullOrEmpty(content))
                        {
                            Interlocked.Increment(ref count);
                        }
                    }
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            count.Should().Be(3);
        }

        [Fact]
        public async Task EndToEnd_ProcessAsBytesAsync_Works()
        {
            int count = 0;
            long totalBytes = 0;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/different_extensions")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".log")
                .ProcessAsBytesAsync(async (f, bytes) =>
                {
                    await Task.Yield(); // Simulate some async work
                    Interlocked.Increment(ref count);
                    Interlocked.Add(ref totalBytes, bytes.Length);
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            count.Should().Be(3);
            totalBytes.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task EndToEnd_FolderWithInvalidZip_HandleException_Succeeds_Async()
        {
            int count = 0;
            bool invalidZipEncountered = false;

            var builder =
                new FileProcessorBuilder()
                .FromFolder("./data/invalid_zip", FolderSelect.RootAndSubFolders)
                .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
                .ProcessAsTextAsync(async (f, t) =>
                {
                    await Task.Yield(); // Simulate some async work
                    Interlocked.Increment(ref count);
                })
                .HandleExceptionWith((f, ex) =>
                {
                    if (f.Extension == ".zip" && ex is InvalidOperationException)
                    {
                        // ignore these exceptions for zip files (corrupted zip)
                        invalidZipEncountered = true;
                        return true;
                    }
                    return false;
                });

            // Act
            await builder.Build().ProcessFilesAsync();

            // Assert
            count.Should().Be(1);
            invalidZipEncountered.Should().BeTrue();
        }
    }
}