using ArchiveFlow.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ArchiveFlow.Utilities
{
    internal static class IArchiveEntryExtensions
    {
        internal static FileInformation ToFileInformation(this IArchiveEntry zipEntry, DateTime defaultDateTime)
        {
            Guard.AgainstNull(nameof(zipEntry), zipEntry);

            return new FileInformation(
                zipEntry.Key,
                zipEntry.Key.LastIndexOf('.') != -1 ?
                    zipEntry.Key.Substring(zipEntry.Key.LastIndexOf('.')) : string.Empty,
                zipEntry.Size,
                zipEntry.LastModifiedTime ?? defaultDateTime,
                true);
        }
    }
}
