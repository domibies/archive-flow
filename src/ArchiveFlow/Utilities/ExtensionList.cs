using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArchiveFlow.Utilities
{
    public static class ExtensionList
    {
        public static bool ContainsExtension(this IList<string> list, string extension)
        {
            Guard.AgainstNull(nameof(list), list);
            Guard.AgainstNull(nameof(extension), extension);
            Guard.AgainsNullOrWhiteSpace(nameof(extension), extension);
            Guard.ShouldStartWith(nameof(extension), extension, ".");

            return list.Distinct().Contains(extension, StringComparer.OrdinalIgnoreCase);
        }
    }
}
