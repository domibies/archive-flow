using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using FluentAssertions;
using Moq;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ArchiveFlow.Tests.UnitTests
{
    public class IArchiveEntryExtensionsTests
    {
        [Fact]
        public void ToFileInformation_ReturnsCorrectFileInformation()
        {
            // Arrange
            DateTime lastModifiedTime = DateTime.Now;

            var zipEntryMock = new Mock<IArchiveEntry>();
            zipEntryMock.SetupGet(x => x.Key).Returns("test.zip");
            zipEntryMock.SetupGet(x => x.LastModifiedTime).Returns(lastModifiedTime);

            var zipEntry = zipEntryMock.Object;

            // Act
            FileInformation fileInformation = zipEntry.ToFileInformation(lastModifiedTime + TimeSpan.FromDays(1));

            // Assert
            fileInformation.Should().NotBeNull();
            fileInformation.FileName.Should().Be(zipEntry.Key);
            fileInformation.Extension.Should().Be(".zip");
            fileInformation.Size.Should().Be(zipEntry.Size);
            fileInformation.LastModified.Should().Be(zipEntry.LastModifiedTime);
        }

        [Fact]
        public void ToFileInformation_WithoutLastModifiedTime_ReturnsDefaultDateTimeParam()
        {
            // Arrange
            DateTime defaultDateTime = new DateTime(2020, 1, 1);

            var zipEntryMock = new Mock<IArchiveEntry>();
            zipEntryMock.SetupGet(x => x.Key).Returns("test.zip");
            var zipEntry = zipEntryMock.Object;

            // Act
            FileInformation fileInformation = zipEntry.ToFileInformation(defaultDateTime);

            // Assert
            fileInformation.Should().NotBeNull();
            fileInformation.LastModified.Should().Be(defaultDateTime);
        }
    }
}
