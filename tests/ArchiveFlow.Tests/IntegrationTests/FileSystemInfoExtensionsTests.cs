using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveFlow.Utilities;
using FluentAssertions;

namespace ArchiveFlow.Tests.IntegrationTests
{
    public class FileSystemInfoExtensionsTests
    {
        // not a unit test, because of the real interaction with a file
        // (can't mock FileSystemInfo)
        [Fact]
        public void ToFileInformation_ReturnsCorrectFileInformation()
        {
            // Arrange
            var path = Path.GetTempPath();
            var extension = ".txt"; 
            var fileName = $"{Guid.NewGuid()}{extension}"; 
            var filePath = Path.Combine(path, fileName);
            File.WriteAllText(filePath, "test");

            FileInfo fileInfo = new FileInfo(filePath);


            // Act
            var fileInformation = fileInfo.ToFileInformation();

            // Assert
            fileInformation.Should().NotBeNull();
            fileInformation.FileName.Should().Be(fileName);
            fileInformation.Extension.Should().Be(extension);   
            fileInformation.Size.Should().Be(fileInfo.Length);
            fileInformation.LastModified.Should().BeSameDateAs(fileInfo.LastWriteTime); 
        }

    }
}
