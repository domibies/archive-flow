using Xunit;
using ArchiveFlow.FileProcessor;
using System.Reflection;
using FluentAssertions;
using ArchiveFlow.Models;
using System;
using System.Threading.Tasks;

namespace ArchiveFlow.Tests.UnitTests
{
    public class AsyncFileProcessorBuilderTests
    {
        [Fact]
        public void ProcessAsTextAsync_ValidAction_SetsAsyncTextProcessingAction()
        {
            // Arrange
            var builder = new FileProcessorBuilder();
            
            // Act
            builder.ProcessAsTextAsync(async (f, t, ct) => await Task.Yield());
            
            // Get the private field via reflection to verify it was set
            var field = builder.GetType()?.GetField("asyncTextProcessingAction", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = field?.GetValue(builder);
            
            // Assert
            value.Should().NotBeNull();
        }

        [Fact]
        public void ProcessAsStreamAsync_ValidAction_SetsAsyncStreamProcessingAction()
        {
            // Arrange
            var builder = new FileProcessorBuilder();
            
            // Act
            builder.ProcessAsStreamAsync(async (f, s, ct) => await Task.Yield());
            
            // Get the private field via reflection to verify it was set
            var field = builder.GetType()?.GetField("asyncStreamProcessingAction", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = field?.GetValue(builder);
            
            // Assert
            value.Should().NotBeNull();
        }

        [Fact]
        public void ProcessAsBytesAsync_ValidAction_SetsAsyncBytesProcessingAction()
        {
            // Arrange
            var builder = new FileProcessorBuilder();
            
            // Act
            builder.ProcessAsBytesAsync(async (f, b, ct) => await Task.Yield());
            
            // Get the private field via reflection to verify it was set
            var field = builder.GetType()?.GetField("asyncBytesProcessingAction", BindingFlags.NonPublic | BindingFlags.Instance);
            var value = field?.GetValue(builder);
            
            // Assert
            value.Should().NotBeNull();
        }

        [Fact]
        public void ProcessAsTextAsync_NullAction_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processAsTextAsync = () => builder.ProcessAsTextAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processAsTextAsync).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessAsStreamAsync_NullAction_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processAsStreamAsync = () => builder.ProcessAsStreamAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processAsStreamAsync).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ProcessAsBytesAsync_NullAction_ThrowsArgumentNullException()
        {
            var builder = new FileProcessorBuilder();
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action processAsBytesAsync = () => builder.ProcessAsBytesAsync(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            // Act & Assert
            FluentActions.Invoking(processAsBytesAsync).Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Build_WithAsyncTextAction_ReturnsFileProcessorInstance()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .ProcessAsTextAsync(async (f, t, ct) => await Task.Yield());

            // Act
            var processor = builder.Build();

            // Assert
            processor.Should().NotBeNull();
        }

        [Fact]
        public void Build_WithMixedSyncAndAsync_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .ProcessAsText((f, t) => { })
                .ProcessAsTextAsync(async (f, t, ct) => await Task.Yield());

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>()
                .WithMessage("*cannot mix sync and async*");
        }

        [Fact]
        public void Build_WithMultipleAsyncActions_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInArchivesOnly)
                .ProcessAsTextAsync(async (f, t, ct) => await Task.Yield())
                .ProcessAsBytesAsync(async (f, b, ct) => await Task.Yield());

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>()
                .WithMessage("*more than one ProcessAsXXX*");
        }

        [Fact]
        public void Build_WhenNoAsyncOrSyncProcessingActionDefined_ThrowsInvalidOperationException()
        {
            // Arrange
            FileProcessorBuilder builder = new FileProcessorBuilder()
                .FromFolder("path")
                .SetArchiveSearch(ArchiveSearch.SearchInAndOutsideArchives)
                .WithExtension(".txt");

            // Act & Assert
            FluentActions.Invoking(() => builder.Build()).Should().Throw<InvalidOperationException>()
                .WithMessage("*no ProcessAsXXX() or ProcessAsXXXAsync() defined*");
        }
    }
}