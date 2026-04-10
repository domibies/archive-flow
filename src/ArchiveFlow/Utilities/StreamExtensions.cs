using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveFlow.Utilities
{
    internal static class StreamExtensions
    {
        public static byte[] ReadAllBytes(this Stream stream)
        {
            const int bufferSize = 4096;
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, count);
                }
                return memoryStream.ToArray();
            }
        }

        public static async Task<byte[]> ReadAllBytesAsync(this Stream stream, CancellationToken cancellationToken = default)
        {
            const int bufferSize = 4096;
            using (var memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[bufferSize];
                int count;
                while ((count = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                {
                    await memoryStream.WriteAsync(buffer, 0, count, cancellationToken).ConfigureAwait(false);
                }
                return memoryStream.ToArray();
            }
        }
    }
}
