using ArchiveFlow.Models;

namespace ArchiveFlow.Utilities
{
    internal static class FileSourceTypeExtensions
    {
        public static bool IncludesZipped(this FileSourceType? sourceType)
        {
            Guard.AgainstNull(nameof(sourceType), sourceType);

            return ((FileSourceType)sourceType).IncludesZipped();
        }

        public static bool IncludesZipped(this FileSourceType sourceType)
        { 
            return sourceType == FileSourceType.Zipped || sourceType == FileSourceType.ZippedAndUnzipped;
        }

        public static bool IncludesUnzipped(this FileSourceType sourceType)
        {
            return sourceType == FileSourceType.Unzipped || sourceType == FileSourceType.ZippedAndUnzipped;
        }
    }
}
