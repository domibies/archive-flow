using ArchiveFlow.Models;
using SharpCompress.Archives;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveFlow.Utilities
{
    public static class IArchiveEntryExtensions
    {
        public static FileInformation ToFileInformation(this IArchiveEntry zipEntry, DateTime defaultDateTime)
        {
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
