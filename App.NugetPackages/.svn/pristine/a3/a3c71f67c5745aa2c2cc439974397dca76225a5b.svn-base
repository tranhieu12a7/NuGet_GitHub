using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.Plugin.Abstractions
{
    public interface IDownloadFile
    {
        event EventHandler<FileResultStatus> FileDownloadCallback;
        event EventHandler<FileResultProgress> FileDownloadProgress;
        string Url { get; }
        string MimeType { get; }
        string FileName { get; }
        string DestinationPathName { get; }
        IDictionary<string, string> Headers { get; }
        FileStatus Status { get; }
        string StatusDetails { get; }
        int TotalRequestException { get; }
    }
}
