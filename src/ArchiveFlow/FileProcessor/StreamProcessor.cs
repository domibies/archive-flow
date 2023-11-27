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
    }
}
