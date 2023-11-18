using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using Moq;
using System;
using System.IO;
using Xunit;
using FluentAssertions;
using SharpCompress.Archives;

namespace ArchiveFlow.Tests.Utilities
{
    public class FileSystemInfoExtensionsTests
    {
        [Fact]
        public void ToFileInformation_NullEntry_ThrowsException()
        {
            // Arrange
            IArchiveEntry? zipEntry = null;
            DateTime defaultDateTime = DateTime.Now;

            // Act
            var action = new Action(() => zipEntry!.ToFileInformation(defaultDateTime));    

            // Assert
            FluentActions.Invoking(action).Should().Throw<ArgumentNullException>(); 
        }
    }
}
