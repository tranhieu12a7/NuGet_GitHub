using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.Plugin.Abstractions
{
    public class FileResultProgress
    {
        public float TotalBytes { get; }
        public float TotalLength { get; }
        public float Percentage { get { return TotalLength > 0 ? 100.0f * (TotalBytes / TotalLength) : 0.0f; } }

        public FileResultProgress(float totalBytes, float totalLength)
        {
            TotalBytes = totalBytes;
            TotalLength = totalLength;
        }
    }
}
