using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreFoundation;
using FileManager.Plugin.Abstractions;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FileManager.Plugin
{
    public class UrlSessionDownloadDelegate : NSUrlSessionDownloadDelegate
    {
        public FileManagerImplementation Controller;
        protected DownloadFileImplementation GetDownloadFileByTask(NSUrlSessionTask downloadTask)
        {
            return Controller.Queue_Download
                .Cast<DownloadFileImplementation>()
                .FirstOrDefault(
                    i => i.Task != null &&
                    (int)i.Task.TaskIdentifier == (int)downloadTask.TaskIdentifier
                );
        }
        /**
         * A Task was resumed (or started ..)
         */

        public override void DidResume(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long resumeFileOffset, long expectedTotalBytes)
        {
            var file = GetDownloadFileByTask(downloadTask);
            if (file == null)
            {
                downloadTask.Cancel();
                return;
            }

            file.Status = FileStatus.RUNNING;
        }

        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            var file = GetDownloadFileByTask(task);
            if (file == null)
                return;

            if (error == null)
                return;

            file.StatusDetails = error.LocalizedDescription;
            file.Status = FileStatus.FAILED;
            file.OnFileDownloadCallback();
        }

        /**
         * The Task keeps receiving data. Keep track of the current progress ...
         */
        public override void DidWriteData(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, long bytesWritten, long totalBytesWritten, long totalBytesExpectedToWrite)
        {
            var file = GetDownloadFileByTask(downloadTask);
            if (file == null)
            {
                downloadTask.Cancel();
                return;
            }

            file.Status = FileStatus.RUNNING;
            file.OnFileDownloadProgress(totalBytesWritten, totalBytesExpectedToWrite);
        }

        public override void DidFinishDownloading(NSUrlSession session, NSUrlSessionDownloadTask downloadTask, NSUrl location)
        {
            Console.WriteLine("File downloaded in : {0}", location);
            var file = GetDownloadFileByTask(downloadTask);
            if (file == null)
            {
                downloadTask.Cancel();
                return;
            }

            // On iOS 9 and later, this method is called even so the response-code is 400 or higher. See https://github.com/cocos2d/cocos2d-x/pull/14683
            var response = downloadTask.Response as NSHttpUrlResponse;
            if (response != null && response.StatusCode >= 400)
            {
                file.StatusDetails = "Error.HttpCode: " + response.StatusCode;
                file.Status = FileStatus.FAILED;
                file.OnFileDownloadCallback();
                return;
            }

            string destinationPathName = MoveDownloadedFile(file, location);
            // If the file destination is unknown or was moved successfully ...
            if (!string.IsNullOrEmpty(destinationPathName))
            {
                file.DestinationPathName = destinationPathName;
                file.Status = FileStatus.COMPLETED;
            }
            else
            {
                file.Status = FileStatus.FAILED;
            }

            file.OnFileDownloadCallback();
        }

        /**
         * Move the downloaded file to it's destination
         */
        public string MoveDownloadedFile(DownloadFileImplementation file, NSUrl location)
        {
            var fileManager = NSFileManager.DefaultManager;
            NSError removeCopy;
            NSError errorCopy;
            string destinationPathName = string.Empty;
            NSUrl destinationURL = Common.GetPathDownloadFile(fileManager, file.FileName);
            try
            {
                fileManager.Remove(destinationURL, out removeCopy);
                bool success = fileManager.Copy(location, destinationURL, out errorCopy);
                if (!success)
                {
                    file.StatusDetails = errorCopy.LocalizedDescription;
                }
                else
                {
                    destinationPathName = destinationURL.Path;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("MoveDownloadedFile:" + ex.Message);
            }
            return destinationPathName;
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