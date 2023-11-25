using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using FluentAssertions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveFlow.Tests.IntegrationTests;
public class EndToEndTests
{
    [Fact]
    public void EndToEnd_Read100x100TxtFiles_Sequential_Works()
    {
        HashSet<int> allContent = new HashSet<int>();

        //Prepare
        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/txt10000")
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .WithExtension(".txt")
            .ProcessAsText((t) =>
            {
                var contentAsInt = int.Parse(t);
                allContent.Add(contentAsInt);
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        // In the dictionary we expect all the numbers from 1 to 10000
        // because we have 10000 files with 1 number each, spread over 100 zip files
        allContent.Should().BeEquivalentTo(Enumerable.Range(1, 10000));

    }

    [Fact]
    public void EndToEnd_Read100x100TxtFiles_InParalllel_Works()
    {
        // MT safe.....
        var concurrentHashSet = new ConcurrentDictionary<int, byte>();

        //Prepare
        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/txt10000")
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .WithExtension(".txt")
            .ProcessAsText((t) =>
            {
                var contentAsInt = int.Parse(t);
                concurrentHashSet.TryAdd(contentAsInt, 0);
            })
            .WithMaxDegreeOfParallelism(Environment.ProcessorCount);

        // Act
        builder.Build().ProcessFiles();

        // Assert
        concurrentHashSet.Keys.Should().BeEquivalentTo(Enumerable.Range(1, 10000));

    }

    [Theory]
    [InlineData(ArchiveSearch.SearchInArchivesOnly, 3, 0)]
    [InlineData(ArchiveSearch.SearchOutsideArchivesOnly, 0, 2)]
    [InlineData(ArchiveSearch.SearchInAndOutsideArchives, 3, 2)]
    public void EndToEnd_ArchiveSearchInAndOutsideArchives_Works(ArchiveSearch archiveSearch, int expectedCountInside, int expectedCountOutside)
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
            .ProcessAsText((t) =>
            {
                if (t.Contains("inside"))
                {
                    countInside++;
                }
                else if (t.Contains("outside"))
                {
                    countOutside++;
                }
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        countInside.Should().Be(expectedCountInside);
        countOutside.Should().Be(expectedCountOutside);
    }

    [Fact]
    public void EndToEnd_NestedArchives_Work()
    {


        //Prepare
        bool foundText = false;

        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/nested_archives")
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .WithExtension(".txt")
            .ProcessAsText((t) =>
            {
                foundText = t.Contains("just a file");
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        foundText.Should().BeTrue();    
    }

    [Theory]
    [InlineData(new string[] { ".txt" }, 4)]
    [InlineData(new string[] { ".log" }, 3)]
    [InlineData(new string[] { ".txt", ".log" }, 7)]
    public void EndToEnd_DifferentExtensions_Work(string[] extensions, int expectedCount)
    {
        int count = 0;

        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/different_extensions")
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .WithExtension(extensions)
            .ProcessAsText((t) =>
            {
                count++;
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        count.Should().Be(expectedCount);
    }

    [Fact]
    public void EndToEnd_RootAndSubFolders_Works()
    {
        int count = 0;

        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/subfolders_and_filters", FolderSelect.RootAndSubFolders)
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .ProcessAsText((t) =>
            {
                count++;
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        count.Should().Be(401);
    }

    [Fact]
    public void EndToEnd_RootAndSubFolders_FromZipWhere_Works()
    {
        int count = 0;

        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/subfolders_and_filters", FolderSelect.RootAndSubFolders)
            .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
            .FromZipWhere((f) => f.FileName.StartsWith("group1"))
            .ProcessAsText((t) =>
            {
                count++;
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        count.Should().Be(300);
    }

    [Fact]
    public void EndToEnd_RootAndSubFolders_WhereFile_Works()
    {
        int count = 0;

        var builder =
            new FileProcessorBuilder()
            .FromFolder("./data/subfolders_and_filters", FolderSelect.RootAndSubFolders)
            .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
            .WhereFile((f) => f.FileName.StartsWith("woohoo"))
            .ProcessAsText((t) =>
            {
                count++;
            });

        // Act
        builder.Build().ProcessFiles();

        // Assert
        count.Should().Be(2);
    }
}
