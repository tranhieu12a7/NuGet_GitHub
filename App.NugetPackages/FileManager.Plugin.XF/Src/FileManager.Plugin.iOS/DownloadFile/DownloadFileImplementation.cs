using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileManager.Plugin.Abstractions;
using Foundation;
using UIKit;

namespace FileManager.Plugin
{
    public class DownloadFileImplementation : IDownloadFile
    {
        public NSUrlSessionTask Task;
        public event EventHandler<FileResultStatus> FileDownloadCallback;
        public event EventHandler<FileResultProgress> FileDownloadProgress;
        public string Url { get; }
        public string MimeType { get; set; }
        public string FileName { get; }

        public string DestinationPathName { get; set; }

        public IDictionary<string, string> Headers { get; }

        public FileStatus Status { get; set; }

        public string StatusDetails { get; set; }

        public int TotalRequestException { get; set; }

        public DownloadFileImplementation(string url, IDictionary<string, string> headers, string fileName)
        {
            FileName = (string.IsNullOrEmpty(fileName)) ? url.Split("/").Last() : fileName;
            Url = url.Replace(" ", "%20");
            Headers = headers;
        }
        public DownloadFileImplementation(NSUrlSessionTask task)
        {
            Url = task.OriginalRequest.Url.AbsoluteString;
            Headers = new Dictionary<string, string>();

            foreach (var header in task.OriginalRequest.Headers)
            {
                Headers.Add(new KeyValuePair<string, string>(header.Key.ToString(), header.Value.ToString()));
            }

            switch (task.State)
            {
                case NSUrlSessionTaskState.Running:
                    Status = FileStatus.RUNNING;
                    break;

                case NSUrlSessionTaskState.Completed:
                    Status = FileStatus.COMPLETED;
                    break;

                case NSUrlSessionTaskState.Canceling:
                    Status = FileStatus.RUNNING;
                    break;

                case NSUrlSessionTaskState.Suspended:
                    Status = FileStatus.PAUSED;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Task = task;
        }

        public void OnFileDownloadProgress(float totalBytes, float totalLength)
        {
            NSOperationQueue.MainQueue.InvokeOnMainThread(() =>
            {
                var fileUploadProgress = new FileResultProgress(totalBytes, totalLength);
                FileDownloadProgress(this, fileUploadProgress);
            });
        }

        public void OnFileDownloadCallback()
        {
            NSOperationQueue.MainQueue.InvokeOnMainThread(() =>
            {
                var fileUploadResponse = new FileResultStatus(this.StatusDetails, this.Status);
                FileDownloadCallback(this, fileUploadResponse);
                (CrossFileManager.Current as FileManagerImplementation).RemoveFileDownload(this);
            });
        }
        public void OnFileDownloadUpdateStatus()
        {
            NSOperationQueue.MainQueue.InvokeOnMainThread(() =>
            {
                var fileUploadResponse = new FileResultStatus(this.StatusDetails, this.Status);
                FileDownloadCallback(this, fileUploadResponse);
            });
        }
        public void StartDownload(NSUrlSession session, bool allowsCellularAccess)
        {
            using (var downloadUrl = NSUrl.FromString(Url))
            using (var request = new NSMutableUrlRequest(downloadUrl))
            {
                if (Headers != null)
                {
                    var headers = new NSMutableDictionary();
                    foreach (var header in Headers)
                    {
                        headers.SetValueForKey(
                            new NSString(header.Value),
                            new NSString(header.Key)
                        );
                    }
                    request.Headers = headers;
                }

                request.AllowsCellularAccess = allowsCellularAccess;
                request.TimeoutInterval = 10;
                Task = session.CreateDownloadTask(request);
                Task.Resume();
            }
        }
    }
}