using ArchiveFlow.FileProcessor;
using FluentAssertions;

namespace ArchiveFlow.Tests.FileProcessor
{
    public class SharpCompressZipFileProcessorTests
    {
        [Fact]
        public void ProcessZipFile_AllFilesProcessed_CounterShouldBe1000()
        {
            // Arrange
            var pathToData = Path.Combine(Directory.GetCurrentDirectory(), @"../../../data");
            var zipFilePath = Path.Combine(pathToData, "filexxx_1000xXML.zip");
            
            int counter = 0;

            var processor = new SharpCompressZipFileProcessor(
                [".xml"],
                (f) => f.FileName.Contains("file"),
                null,
                (t) => counter++,
                null);

            // Act
            processor.ProcessZipFile(new FileInfo(zipFilePath));

            // Assert
            counter.Should().Be(1000);
        }
    }
}
