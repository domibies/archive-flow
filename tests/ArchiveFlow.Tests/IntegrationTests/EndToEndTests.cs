using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using FluentAssertions;
using System;
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

        allContent.Should().BeEquivalentTo(Enumerable.Range(1, 10000));

    }
}
