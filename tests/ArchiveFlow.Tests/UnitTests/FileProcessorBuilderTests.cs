using Xunit;
using Moq;
using System.IO;
using ArchiveFlow.FileProcessor;
using System.Reflection;
using FluentAssertions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;

namespace ArchiveFlow.Tests.UnitTests
{
    public class FileProcessorBuilderTests
    {

        [Fact]
        public void FromFolder_ValidPath_SetsFolderPath()
        {
            // Arrange
            var builder = new FileProcessorBuilder();
            var expectedPath = "testPath";

            // Act
            builder.FromFolder(expectedPath);
            // get private folderPath from builder
            var actualPath = builder.GetType()?.GetField("folderPath", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(builder);

            // Assert
            actualPath.Should().Be(expectedPath);
        }

        [Fact]
        public void FromFolder_NullPath_ThrowsArgumentNullException()
        {
            // Arrange
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action fromFolder = () => builder.FromFolder(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(fromFolder).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Where_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action where = () => builder.Where(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(where).Should().Throw<ArgumentNullException>();
        }

        // test that WhereZip throws
        [Fact]
        public void WhereZip_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action whereZip = () => builder.WhereZip(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(whereZip).Should().Throw<ArgumentNullException>();
        }

        // ProcessStreamWith

        [Fact]
        public void ProcessStreamWith_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processStreamWith = () => builder.ProcessStreamWith(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processStreamWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessStreamWith_AfterOtherProcess_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessTextWith((t) => { });
            Action processStreamWith = () => builder.ProcessStreamWith((s) => { });

            // Act & Assert
            FluentActions.Invoking(processStreamWith).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ProcessTextWith_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processTextWith = () => builder.ProcessTextWith(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processTextWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessTextWith_AfterOtherProcessWith_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessBytesWith((b) => { });
            Action processTextWith = () => builder.ProcessTextWith((t) => { });

            // Act & Assert
            FluentActions.Invoking(processTextWith).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void ProcessBytesWith_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processBytesWith = () => builder.ProcessBytesWith(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processBytesWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessBytesWith_AfterOtherProcessWith_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessStreamWith((s) => { });
            Action processBytesWith = () => builder.ProcessBytesWith((t) => { });

            // Act & Assert
            FluentActions.Invoking(processBytesWith).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Build_WhenFolderPathIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenNoProcessXXXWithDefined_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .UseSource(FileSourceType.ZippedAndUnzipped)
                .FilterByExtension(".txt");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenNoUseSourceDefined_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .FilterByExtension(".txt")
                .ProcessTextWith((t) => { });

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenValidParameters_ReturnsFileProcessorInstance()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .UseSource(FileSourceType.ZippedAndUnzipped)
                .ProcessTextWith((t) => { });

            // Act
            var processor = builder.Build();

            // Assert
            processor.Should().NotBeNull();
        }

    }
}