using System;
using System.Collections.Generic;
using System.Text;

namespace FileManager.Plugin.Abstractions
{
    public interface IUploadFile
    {
        event EventHandler<FileResultStatus> FileUploadCallback;
        event EventHandler<FileResultProgress> FileUploadProgress;
        string Url { get; }
        string FileName { get; }
        string Path { get; }
        byte[] Bytes { get; }
        IDictionary<string, string> Headers { get; }
        IDictionary<string, string> Parameters { get; }
        string Boundary { get; }
        string DataResult { get; }
        FileStatus StatusCode { get; }
    }
}
