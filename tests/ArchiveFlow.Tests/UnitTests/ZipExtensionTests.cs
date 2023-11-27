using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using FluentAssertions;
using System;
using Xunit;


namespace ArchiveFlow.Tests.UnitTests
{
    public class ZipExtensionTests
    {
        [Theory]
        [InlineData("zip")]
        public void IsZipExtension_InvalidExtension_ThrowsArgumentException(string extension)
        {
            // Arrange

            // Act
            Action action = () => extension.IsZipExtension();

            // Assert
            FluentActions.Invoking(action).Should().Throw<ArgumentException>();
        }


        [Theory]
        [InlineData(".zip")]
        [InlineData(".rar")]
        [InlineData(".7z")]
        public void IsZipExtension_ZipExtension_ReturnsTrue(string extension)
        {
            // Arrange

            // Act
            bool result = extension.IsZipExtension();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsZipExtension_NonZipExtension_ReturnsFalse()
        {
            // Arrange
            string extension = ".txt";

            // Act
            bool result = extension.IsZipExtension();

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(".zip")]
        [InlineData(".rar")]
        [InlineData(".7z")]
        public void IsZipFile_IsZipExtension_ReturnsTrue(string extension)
        {
            // Arrange
            FileInformation file = new FileInformation("test", "test", extension, 0, DateTime.Now, false);

            // Act
            bool result = file.IsZipFile();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsZipFile_IsNotZipExtension_ReturnsFalse()
        {
            // Arrange
            FileInformation file = new FileInformation("test", "test", ".text", 0, DateTime.Now, false);

            // Act
            bool result = file.IsZipFile();

            // Assert
            result.Should().BeFalse();
        }
    }
}
