using System;
using System.Collections.Generic;
using System.IO;

namespace Tracker.Pixel.Service.Utilities
{
    public static class FileCache
    {
        private static readonly object _lock = new object();
        private static readonly Dictionary<string, byte[]> _cache = new Dictionary<string, byte[]>();

        public static byte[] ReadFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentException("File path cannot be null or empty.");
            }

            if (!_cache.TryGetValue(filePath, out var cachedBytes))
            {
                lock (_lock)
                {
                    if (!_cache.TryGetValue(filePath, out cachedBytes))
                    {
                        cachedBytes = File.ReadAllBytes(filePath);
                        _cache[filePath] = cachedBytes;
                    }
                }
            }

            return cachedBytes;
        }
    }
}
