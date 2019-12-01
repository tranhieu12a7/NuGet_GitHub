using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace FileManager.Plugin
{
    public interface ICountProgressListener
    {
        void OnFileUploadProgress(long bytesWritten, long contentLength);
        void OnFileUploadCallback(string error);
        void OnFileUploadError(string error);
    }
}