using ArchiveFlow.Abstractions;
using ArchiveFlow.Common;
using ArchiveFlow.Models;
using ArchiveFlow.Utilities;
using System;
using System.IO;

namespace ArchiveFlow.FileProcessor
{
    internal class StreamProcessor : IStreamProcessor
    {
        private readonly StreamProcessingAction? streamProcessingAction;
        private readonly TextProcessingAction? textProcessingAction;
        private readonly BytesProcessingAction? bytesProcessingAction;

        public StreamProcessor(
            StreamProcessingAction? streamProcessingAction,
            TextProcessingAction? textProcessingAction,
            BytesProcessingAction? bytesProcessingAction)
        {
            this.streamProcessingAction = streamProcessingAction;
            this.textProcessingAction = textProcessingAction;
            this.bytesProcessingAction = bytesProcessingAction;
        }

        public void ProcessStream(Stream stream)
        {
            Guard.AgainstNull(nameof(stream), stream);

            MaybeProcessStream(stream);
            MaybeProcessText(stream);
            MaybeProcessBytes(stream);
        }

        public void MaybeProcessStream(Stream stream)
        {
            if (!(streamProcessingAction is null))
            {
                streamProcessingAction?.Invoke(stream);
            }
        }

        public void MaybeProcessText(Stream stream)
        {
            if (!(textProcessingAction is null))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    textProcessingAction?.Invoke(reader.ReadToEnd());
                }
            }
        }

        public void MaybeProcessBytes(Stream stream)
        {
            if (!(bytesProcessingAction is null))
            {
                bytesProcessingAction?.Invoke(stream.ReadAllBytes());
            }
        }
    }
}
