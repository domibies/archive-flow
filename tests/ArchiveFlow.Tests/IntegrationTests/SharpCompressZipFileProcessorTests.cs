using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using FluentAssertions;
using System.Text.RegularExpressions;
using Xunit;
using System.IO;
using System.Collections.Generic;
using System.Linq;


namespace ArchiveFlow.Tests.IntegrationTests
{
    public class SharpCompressZipFileProcessorTests
    {
        const string zipFile_1000xXML = "filexxx_1000xXML.zip";
        const int totalNumberOfFiles_1000xXML = 1000;
        const string zipFile_2xJPG = "img_2xjpg.7z";
        const int totalSize_2xJPG = 2079 + 1011;


        [Fact]
        public void ProcessZipFile_AllFilesProcessed_CounterOK()
        {
            // Arrange
            var zipFilePath = Path.Combine("./data", zipFile_1000xXML);

            int counter = 0;

            var config = new ProcessorConfig("./data", FolderSelect.RootAndSubFolders, ArchiveSearch.SearchInArchivesOnly, (new List<string> {".xml"}).AsEnumerable<string>(), null, null, null, (f, t) => counter++, null, null, 1);
            SharpCompressZipFileProcessor processor = new SharpCompressZipFileProcessor(config);

            // Act
            processor.ProcessZipFile(new FileInfo(zipFilePath).ToFileInformation());

            // Assert
            counter.Should().Be(totalNumberOfFiles_1000xXML);
        }

        [Fact]
        public void ProcessZipFile_EvenFilesProcessed_CounterOK()
        {
            // Arrange
            var zipFilePath = Path.Combine("./data", zipFile_1000xXML);

            int counter = 0;

            var config = new ProcessorConfig("./data", FolderSelect.RootAndSubFolders, ArchiveSearch.SearchInArchivesOnly, new List<string> { ".xml" },
                (f) =>
                {
                    bool isEven = int.TryParse(Regex.Match(f.FileName, @"\d+").Value, out int number) && number % 2 == 0;
                    return isEven;
                }
            , null, null, (f, t) => counter++, null, null, 1);

            SharpCompressZipFileProcessor processor = new SharpCompressZipFileProcessor(config);

            // Act
            processor.ProcessZipFile(new FileInfo(zipFilePath).ToFileInformation());

            // Assert
            counter.Should().Be(totalNumberOfFiles_1000xXML / 2);
        }

        [Fact]
        public void ProcessZipFile_7ZipWith2Jpg_Works()
        {
            var zipFilePath = Path.Combine("./data", zipFile_2xJPG);

            int counter = 0;
            int byteSize = 0;

            var config = new ProcessorConfig("./data", FolderSelect.RootAndSubFolders, ArchiveSearch.SearchInArchivesOnly, new List<string> { ".jpg" }, null, null, null, null, (f, b) => { counter++; byteSize += b.Length; }, null, 1);

            var processor = new SharpCompressZipFileProcessor(config);

            // Act
            processor.ProcessZipFile(new FileInfo(zipFilePath).ToFileInformation());

            // Assert
            counter.Should().Be(2);
            byteSize.Should().Be(2079 + 1011);
        }

    }
}
