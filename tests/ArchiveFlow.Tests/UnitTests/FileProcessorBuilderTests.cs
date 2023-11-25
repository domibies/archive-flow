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
        public void Where_Filter_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action where = () => builder.WhereFile(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(where).Should().Throw<ArgumentNullException>();
        }

        // test that WhereZip throws
        [Fact]
        public void FromZipWhere_Filter_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action whereZip = () => builder.FromZipWhere(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(whereZip).Should().Throw<ArgumentNullException>();
        }

        // ProcessStreamWith

        [Fact]
        public void SetStreamProcessingAction_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processStreamWith = () => builder.ProcessAsText(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processStreamWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetStreamProcessingAction_AfterOtherProcess_Build_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessAsText((t) => { });
            builder.ProcessAsStream((s) => { });

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SetTextProcessingAction_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processTextWith = () => builder.ProcessAsText(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processTextWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetTextProcessingAction_AfterOtherProcessWith_Build_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessAsBytes((b) => { });
            builder.ProcessAsText((t) => { });

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void SetBytesProcessingAction_NullPath_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processBytesWith = () => builder.ProcessAsBytes(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processBytesWith).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetBytesProcessingAction_AfterOtherProcessWith_ThrowsInvalidOperationException()
        {
            var builder = new FileProcessorBuilder();
            builder.ProcessAsText((s) => { });
            builder.ProcessAsBytes((t) => { });

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Build_WhenFolderPathIsNull_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder();

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Build_WhenNoProcessingActionDefined_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
                .WithExtension(".txt");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Build_WhenNoSourceDefined_ShouldntThrow()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .WithExtension(".txt")
                .ProcessAsText((t) => { });

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().NotThrow();
        }

        [Fact]
        public void Build_WhenValidParameters_ReturnsFileProcessorInstance()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .ProcessAsText((t) => { });

            // Act
            var processor = builder.Build();

            // Assert
            processor.Should().NotBeNull();
        }

    }
}