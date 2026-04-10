using ArchiveFlow.Abstractions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveFlow.FileProcessor
{
    internal class StreamProcessor : IStreamProcessor
    {
        private readonly StreamProcessingAction? streamProcessingAction;
        private readonly TextProcessingAction? textProcessingAction;
        private readonly BytesProcessingAction? bytesProcessingAction;
        private readonly AsyncStreamProcessingAction? asyncStreamProcessingAction;
        private readonly AsyncTextProcessingAction? asyncTextProcessingAction;
        private readonly AsyncBytesProcessingAction? asyncBytesProcessingAction;

        public StreamProcessor(
            StreamProcessingAction? streamProcessingAction,
            TextProcessingAction? textProcessingAction,
            BytesProcessingAction? bytesProcessingAction)
        {
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
        }

        public StreamProcessor(
            AsyncStreamProcessingAction? asyncStreamProcessingAction,
            AsyncTextProcessingAction? asyncTextProcessingAction,
            AsyncBytesProcessingAction? asyncBytesProcessingAction)
        {
            this.asyncStreamProcessingAction = asyncStreamProcessingAction;
            this.asyncTextProcessingAction = asyncTextProcessingAction;
            this.asyncBytesProcessingAction = asyncBytesProcessingAction;
        }

        public void ProcessStream(FileInformation file, Stream stream)
        {
            Guard.AgainstNull(nameof(stream), stream);

            MaybeProcessStream(file,stream);
            MaybeProcessText(file,stream);
            MaybeProcessBytes(file, stream);
        }

        public void MaybeProcessStream(FileInformation file, Stream stream)
        {
            if (!(streamProcessingAction is null))
            {
                streamProcessingAction?.Invoke(file,stream);
            }
        }

        public void MaybeProcessText(FileInformation file, Stream stream)
        {
            if (!(textProcessingAction is null))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    textProcessingAction?.Invoke(file, reader.ReadToEnd());
                }
            }
        }

        public void MaybeProcessBytes(FileInformation file, Stream stream)
        {
            if (!(bytesProcessingAction is null))
            {
                bytesProcessingAction?.Invoke(file, stream.ReadAllBytes());
            }
        }

        public async Task ProcessStreamAsync(FileInformation file, Stream stream, CancellationToken cancellationToken = default)
        {
            Guard.AgainstNull(nameof(stream), stream);

            await MaybeProcessStreamAsync(file, stream, cancellationToken).ConfigureAwait(false);
            await MaybeProcessTextAsync(file, stream, cancellationToken).ConfigureAwait(false);
            await MaybeProcessBytesAsync(file, stream, cancellationToken).ConfigureAwait(false);
        }

        public async Task MaybeProcessStreamAsync(FileInformation file, Stream stream, CancellationToken cancellationToken = default)
        {
            if (!(asyncStreamProcessingAction is null))
            {
                await asyncStreamProcessingAction(file, stream, cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task MaybeProcessTextAsync(FileInformation file, Stream stream, CancellationToken cancellationToken = default)
        {
            if (!(asyncTextProcessingAction is null))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    var content = await reader.ReadToEndAsync().ConfigureAwait(false);
                    await asyncTextProcessingAction(file, content, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        public async Task MaybeProcessBytesAsync(FileInformation file, Stream stream, CancellationToken cancellationToken = default)
        {
            if (!(asyncBytesProcessingAction is null))
            {
                var bytes = await stream.ReadAllBytesAsync(cancellationToken).ConfigureAwait(false);
                await asyncBytesProcessingAction(file, bytes, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
