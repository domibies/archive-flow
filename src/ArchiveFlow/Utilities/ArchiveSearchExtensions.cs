using ArchiveFlow.Models;

namespace ArchiveFlow.Utilities
{
    internal static class ArchiveSearchExtensions
    {
        public static bool IncludesZipped(this ArchiveSearch? archiveSearch)
        {
            Guard.AgainstNull(nameof(archiveSearch), archiveSearch);

            return ((ArchiveSearch)archiveSearch!).IncludesZipped();
        }

        public static bool IncludesZipped(this ArchiveSearch archiveSearch)
        { 
            return archiveSearch == ArchiveSearch.SearchInArchivesOnly || archiveSearch == ArchiveSearch.SearchInAndOutsideArchives;
        }

        public static bool IncludesUnzipped(this ArchiveSearch archiveSearch)
        {
            return archiveSearch == ArchiveSearch.SearchOutsideArchivesOnly || archiveSearch == ArchiveSearch.SearchInAndOutsideArchives;
        }
    }
}
