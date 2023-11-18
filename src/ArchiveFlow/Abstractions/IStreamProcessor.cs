
using System.IO;

namespace ArchiveFlow.Abstractions
{
    internal interface IStreamProcessor
    {
        void ProcessStream(Stream stream);
        void MaybeProcessStream(Stream stream);
        void MaybeProcessText(Stream stream);
        void MaybeProcessBytes(Stream stream);
    }
}
