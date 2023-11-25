using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using Moq;
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using SharpCompress.Archives;

namespace ArchiveFlow.Tests.UnitTests
{
    public class FileSystemInfoExtensionsTests
    {
        [Fact]
        public void ToFileInformation_NullEntry_ThrowsException()
        {
            // Arrange
            IArchiveEntry? zipEntry = null;
            DateTime defaultDateTime = DateTime.Now;
            var parentFile = new FileInformation("test", "test.zip", ".zip", 0, defaultDateTime, false);

            // Act
            var action = new Action(() => zipEntry!.ToFileInformation(parentFile));

            // Assert
            FluentActions.Invoking(action).Should().Throw<ArgumentNullException>();
        }
    }
}
