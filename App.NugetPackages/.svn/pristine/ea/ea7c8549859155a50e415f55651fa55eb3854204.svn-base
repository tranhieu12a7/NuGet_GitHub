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
using FileManager.Plugin.Abstractions;

namespace FileManager.Plugin
{
    public class DownloadFileImplementation : IDownloadFile
    {
        public event EventHandler<FileResultStatus> FileDownloadCallback;
        public event EventHandler<FileResultProgress> FileDownloadProgress;
        public long Id;
        public string Url { get; }
        public string MimeType { get; set; }
        public string FileName { get; }
        public string DestinationPathName { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public FileStatus Status { get; set; }
        public string StatusDetails { get; set; }
        public int TotalRequestException { get; set; }

        private Activity activity;
        public DownloadFileImplementation(string url, IDictionary<string, string> headers, string fileName,Activity activity)
        {
            FileName = (string.IsNullOrEmpty(fileName)) ? url.Split("/").Last() : fileName;
            Url = url.Replace(" ", "%20");
            Headers = headers;
            this.activity = activity;
        }
        public void OnFileDownloadProgress(float totalBytes, float totalLength)
        {
            this.activity.RunOnUiThread(() =>
            {
                var fileUploadProgress = new FileResultProgress(totalBytes, totalLength);
                FileDownloadProgress(this, fileUploadProgress);
            });
        }

        public void OnFileDownloadCallback()
        {
            this.activity.RunOnUiThread(() =>
            {
                var fileUploadResponse = new FileResultStatus(this.StatusDetails, this.Status);
                FileDownloadCallback(this, fileUploadResponse);
                (CrossFileManager.Current as FileManagerImplementation).RemoveFileDownload(this);
            });
        }

        public void OnFileDownloadUpdateStatus()
        {
            this.activity.RunOnUiThread(() =>
            {
                var fileUploadResponse = new FileResultStatus(this.StatusDetails, this.Status);
                FileDownloadCallback(this, fileUploadResponse);
            });
        }

        public void StartDownload(Android.App.DownloadManager downloadManager, string destinationPathName,
            bool allowedOverMetered, DownloadVisibility notificationVisibility)
        {
            using (var downloadUrl = Android.Net.Uri.Parse(Url))
            using (var request = new Android.App.DownloadManager.Request(downloadUrl))
            {
                if (Headers != null)
                {
                    foreach (var header in Headers)
                    {
                        request.AddRequestHeader(header.Key, header.Value);
                    }
                }

                if (destinationPathName != null)
                {
                    var file = new Java.IO.File(destinationPathName);
                    var uriPathFile = Android.Net.Uri.FromFile(file);
                    request.SetDestinationUri(uriPathFile);
                    if (file.Exists())
                    {
                        file.Delete();
                    }
                }
                request.SetVisibleInDownloadsUi(true);
                request.SetAllowedOverMetered(allowedOverMetered);
                request.SetNotificationVisibility(notificationVisibility);
                Id = downloadManager.Enqueue(request);
            }
        }
    }
}