using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CoreFoundation;
using FileManager.Plugin.Abstractions;
using Foundation;
using UIKit;

namespace FileManager.Plugin
{
    public class UrlSessionTaskDelegate : NSUrlSessionTaskDelegate
    {
        public FileManagerImplementation Controller;
        protected UploadFileImplementation GetDownloadFileByTask(NSUrlSessionTask downloadTask)
        {
            return Controller.Queue_Upload
                .Cast<UploadFileImplementation>()
                .FirstOrDefault(
                    i => i.Task != null &&
                    (int)i.Task.TaskIdentifier == (int)downloadTask.TaskIdentifier
                );
        }
        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            var file = GetDownloadFileByTask(task);
            if (file == null)
                return;

            Console.WriteLine(string.Format("DidCompleteWithError TaskId: {0}{1}", task.TaskIdentifier, (error == null ? "" : " Error: " + error.Description)));

            var responseError = false;

            string parts = task.TaskDescription;

            if (task.Response is NSHttpUrlResponse)
            {
                var response = task.Response as NSHttpUrlResponse;
                Console.WriteLine("HTTP Response {0}", response);
                Console.WriteLine("HTTP Status {0}", response.StatusCode);
                responseError = response.StatusCode != 200 && response.StatusCode != 201;
            }

            System.Diagnostics.Debug.WriteLine("COMPLETE");

            //Remove the temporal multipart file
            if (!string.IsNullOrEmpty(parts) && File.Exists(parts))
            {
                File.Delete(parts);
            }

            if (error == null && !responseError)
            {
                file.StatusCode = FileStatus.COMPLETED;
                file.StatusDetail = string.Empty;
            }
            else if (responseError)
            {
                file.StatusCode = FileStatus.FAILED;
                file.StatusDetail = "Response Error";
            }
            else
            {
                file.StatusCode = FileStatus.FAILED;
                file.StatusDetail = error.Description;
            }
        }

        //public override void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidReceiveData... MTL");
        //    //if (uploadData.ContainsKey(dataTask.TaskIdentifier))
        //    //{
        //    //    uploadData[dataTask.TaskIdentifier].AppendData(data);
        //    //}
        //    //else
        //    //{
        //    //    var uData = new NSMutableData();
        //    //    uData.AppendData(data);
        //    //    uploadData.Add(dataTask.TaskIdentifier, uData);
        //    //}
        //    // _data.AppendData(data);
        //}
        public override void DidFinishCollectingMetrics(NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics)
        {
            var file = GetDownloadFileByTask(task);
            if (file == null)
                return;
          
            var responseError = true;
            if (task.Response is NSHttpUrlResponse)
            {
                var response = task.Response as NSHttpUrlResponse;
                Console.WriteLine("HTTP Response {0}", response);
                Console.WriteLine("HTTP Status {0}", response.StatusCode);
                responseError = response.StatusCode != 200 && response.StatusCode != 201;
            }

            if (!responseError)
            {
                file.StatusCode = FileStatus.COMPLETED;
                file.StatusDetail = string.Empty;
            }
            else if (responseError)
            {
                file.StatusCode = FileStatus.FAILED;
                file.StatusDetail = "Response Error";
            }
        }
        public override void DidBecomeInvalid(NSUrlSession session, NSError error)
        {
            System.Diagnostics.Debug.WriteLine("DidBecomeInvalid" + (error == null ? "undefined" : error.Description));
        }

        public override void DidSendBodyData(NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
        {
            var file = GetDownloadFileByTask(task);
            if (file == null)
                return;

            string part = task.TaskDescription;

            //file.OnFileUploadProgress(totalBytesSent, totalBytesExpectedToSend);
        }
        public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
        {
            var completionHandler = CrossFileManager.BackgroundSessionCompletionHandler;
            if (completionHandler != null)
            {
                CrossFileManager.BackgroundSessionCompletionHandler = null;

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    completionHandler();
                });
            }
        }
    }
}