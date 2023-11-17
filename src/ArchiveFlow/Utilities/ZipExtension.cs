using ArchiveFlow.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace ArchiveFlow.Utilities
{
    public static class ZipExtension
    {
        private static readonly List<string> zipExtensionsList = new List<string> { ".zip", ".rar", ".7z" };

        public static bool IsZipExtension(this string extension)
        {
            Guard.AgainstNull(nameof(extension), extension);
            Guard.AgainsNullOrWhiteSpace(nameof(extension), extension);
            Guard.ShouldStartWith(nameof(extension), extension, ".");

            return zipExtensionsList.Any(s => s.Equals(extension, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsZipFile(this FileInformation file)
        {
            return file.Extension.IsZipExtension();
        }
    }
}
