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
            .UseSource(FileSourceType.Zipped)
            .FilterByExtension(".txt")
            .ProcessTextWith((t) =>
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
            .UseSource(FileSourceType.Zipped)
            .FilterByExtension(".txt")
            .ProcessTextWith((t) =>
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
}
