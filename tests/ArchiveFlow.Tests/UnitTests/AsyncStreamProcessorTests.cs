using Xunit;
using FluentAssertions;
using ArchiveFlow.FileProcessor;
using ArchiveFlow.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ArchiveFlow.Tests.UnitTests
{
    public class AsyncStreamProcessorTests
    {
        [Fact]
        public async Task ProcessStreamAsync_WithAsyncTextAction_CallsAction()
        {
            // Arrange
            bool actionCalled = false;
            string? capturedText = null;

            var processor = new StreamProcessor(
                null,
                async (f, text) => 
                {
                    await Task.Yield();
                    actionCalled = true;
                    capturedText = text;
                },
                null);

            var fileInfo = new FileInformation("", "test.txt", ".txt", 100, DateTime.Now, false);
            var content = "Hello, World!";
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));

            // Act
            await processor.ProcessStreamAsync(fileInfo, stream);

            // Assert
            actionCalled.Should().BeTrue();
            capturedText.Should().Be(content);
        }

        [Fact]
        public async Task ProcessStreamAsync_WithAsyncBytesAction_CallsAction()
        {
            // Arrange
            bool actionCalled = false;
            byte[]? capturedBytes = null;

            var processor = new StreamProcessor(
                null,
                null,
                async (f, bytes) => 
                {
                    await Task.Yield();
                    actionCalled = true;
                    capturedBytes = bytes;
                });

            var fileInfo = new FileInformation("", "test.bin", ".bin", 100, DateTime.Now, false);
            var content = new byte[] { 1, 2, 3, 4, 5 };
            using var stream = new MemoryStream(content);

            // Act
            await processor.ProcessStreamAsync(fileInfo, stream);

            // Assert
            actionCalled.Should().BeTrue();
            capturedBytes.Should().BeEquivalentTo(content);
        }

        [Fact]
        public async Task ProcessStreamAsync_WithAsyncStreamAction_CallsAction()
        {
            // Arrange
            bool actionCalled = false;
            long streamLength = 0;

            var processor = new StreamProcessor(
                async (f, stream) => 
                {
                    await Task.Yield();
                    actionCalled = true;
                    streamLength = stream.Length;
                },
                null,
                null);

            var fileInfo = new FileInformation("", "test.dat", ".dat", 100, DateTime.Now, false);
            var content = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            using var stream = new MemoryStream(content);

            // Act
            await processor.ProcessStreamAsync(fileInfo, stream);

            // Assert
            actionCalled.Should().BeTrue();
            streamLength.Should().Be(content.Length);
        }

        [Fact]
        public async Task MaybeProcessTextAsync_WithNullAction_DoesNothing()
        {
            // Arrange
            var processor = new StreamProcessor(null, null, null);
            var fileInfo = new FileInformation("", "test.txt", ".txt", 100, DateTime.Now, false);
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("test"));

            // Act & Assert (should not throw)
            await processor.MaybeProcessTextAsync(fileInfo, stream);
        }

        [Fact]
        public async Task MaybeProcessBytesAsync_WithNullAction_DoesNothing()
        {
            // Arrange
            var processor = new StreamProcessor(null, null, null);
            var fileInfo = new FileInformation("", "test.bin", ".bin", 100, DateTime.Now, false);
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

            // Act & Assert (should not throw)
            await processor.MaybeProcessBytesAsync(fileInfo, stream);
        }

        [Fact]
        public async Task MaybeProcessStreamAsync_WithNullAction_DoesNothing()
        {
            // Arrange
            var processor = new StreamProcessor(null, null, null);
            var fileInfo = new FileInformation("", "test.dat", ".dat", 100, DateTime.Now, false);
            using var stream = new MemoryStream(new byte[] { 1, 2, 3 });

            // Act & Assert (should not throw)
            await processor.MaybeProcessStreamAsync(fileInfo, stream);
        }

        [Fact]
        public async Task ProcessStreamAsync_WithNullStream_ThrowsArgumentNullException()
        {
            // Arrange
            var processor = new StreamProcessor(
                async (f, s) => await Task.Yield(), 
                null, 
                null);
            var fileInfo = new FileInformation("", "test.dat", ".dat", 100, DateTime.Now, false);

            // Act & Assert
            await FluentActions.Invoking(async () => await processor.ProcessStreamAsync(fileInfo, null!))
                .Should().ThrowAsync<ArgumentNullException>();
        }
    }
}