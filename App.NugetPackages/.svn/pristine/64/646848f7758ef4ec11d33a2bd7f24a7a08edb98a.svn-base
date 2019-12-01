using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Database;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FileManager.Plugin.Abstractions;

namespace FileManager.Plugin
{
    public class FileManagerImplementation : IFileManager
    {
        #region Properties
        private Android.OS.Handler _watcherDownloadHandler;
        private Java.Lang.Runnable _watcherDownloadHandlerRunnable;

        private readonly Android.App.DownloadManager _downloadManager;
        private int _maximumConnectionsPerQueue;
        private Activity _activity;


        private readonly IList<IDownloadFile> _queue_Download;
        public IEnumerable<IDownloadFile> Queue_Download
        {
            get
            {
                lock (_queue_Download)
                {
                    return _queue_Download.ToList();
                }
            }
        }
        private readonly IList<IUploadFile> _queue_Upload;
        public IEnumerable<IUploadFile> Queue_Upload
        {
            get
            {
                lock (_queue_Upload)
                {
                    return _queue_Upload.ToList();
                }
            }
        }
        public DownloadVisibility NotificationVisibility;

        #endregion
        /// <summary>
        /// truyền số lần download và upload / hàng đợi 
        /// </summary>
        /// <param name="maximumConnectionsPerQueue"></param>
        public void Init(Activity activity, int maximumConnectionsPerQueue)
        {
            _activity = activity;
            _maximumConnectionsPerQueue = maximumConnectionsPerQueue;
        }
        public FileManagerImplementation()
        {
            _queue_Download = new List<IDownloadFile>();
            _queue_Upload = new List<IUploadFile>();
            _downloadManager = (Android.App.DownloadManager)Android.App.Application.Context.GetSystemService(Context.DownloadService);
        }

        #region Download file
        public IDownloadFile CreateFileDownload(string url, string fileName = null)
        {
            return CreateFileDownload(url, new Dictionary<string, string>(), fileName);
        }

        public IDownloadFile CreateFileDownload(string url, IDictionary<string, string> headers, string fileName = null)
        {
            var downloads = Queue_Download.Cast<DownloadFileImplementation>();
            var fileDownload = downloads.Where(x => x.Url == url.Replace(" ", "%20")).FirstOrDefault();
            return (fileDownload != null) ? fileDownload : new DownloadFileImplementation(url, headers, fileName, _activity);
        }
        public void StartDownloadFile(IDownloadFile file)
        {
            var fileDownload = (DownloadFileImplementation)file;
            if (fileDownload.Status == FileStatus.NONE)
            {
                if (FileExistenceCheck(file))
                    return;

                fileDownload.Status = FileStatus.INITIALIZED;

                fileDownload.OnFileDownloadUpdateStatus();
                AddFileDownload(fileDownload);

                StartDownloadWatcher();
            }
            else if (fileDownload.Status == FileStatus.RUNNING)
            {
                fileDownload.Status = FileStatus.RUNNING;
                fileDownload.OnFileDownloadUpdateStatus();
            }
            else
                AbortFileDownload(fileDownload);
        }

        public void AbortDownloadFile(IDownloadFile file)
        {
            var downloadFile = (DownloadFileImplementation)file;

            AbortFileDownload(downloadFile);
        }

        private void StartDownloadWatcher()
        {
            if (_watcherDownloadHandler != null)
                return;

            // Create an instance for a runnable-handler
            _watcherDownloadHandler = new Android.OS.Handler();

            // Create a runnable, restarting itself to update every file in the queue
            _watcherDownloadHandlerRunnable = new Java.Lang.Runnable(() =>
            {
                var queueDownload = Queue_Download.Cast<DownloadFileImplementation>().Take(_maximumConnectionsPerQueue);
                foreach (DownloadFileImplementation file in queueDownload)
                {
                    if (file == null)
                        continue;

                    if (file.Status == FileStatus.INITIALIZED)
                    {
                        string destinationPathName = GetPathDownloadFile(file.FileName);
                        file.StartDownload(_downloadManager, destinationPathName, true, NotificationVisibility);
                    }

                    var query = new Android.App.DownloadManager.Query();
                    query.SetFilterById(file.Id);
                    try
                    {
                        using (var cursor = _downloadManager.InvokeQuery(query))
                        {
                            if (cursor != null && cursor.MoveToNext())
                            {
                                UpdateFileDownloadProperties(cursor, file);
                            }
                            else
                            {
                                // This file is not listed in the native download manager anymore. Let's mark it as canceled.
                                AbortFileDownload(file);
                            }
                            cursor?.Close();
                        }
                    }
                    catch (Android.Database.Sqlite.SQLiteException)
                    {
                        // I lately got an exception that the database was unaccessible ...
                    }

                }
                _watcherDownloadHandler?.PostDelayed(_watcherDownloadHandlerRunnable, 1000);
            });
            // Start this playing handler immediately
            _watcherDownloadHandler.Post(_watcherDownloadHandlerRunnable);
        }
        public void StopDownloadWatcher()
        {
            lock (_queue_Download)
            {
                var downloads = Queue_Download.Cast<DownloadFileImplementation>();
                if (Common.IsAnyData(downloads) || _watcherDownloadHandler == null)
                    return;

                _watcherDownloadHandler.RemoveCallbacks(_watcherDownloadHandlerRunnable);
                _watcherDownloadHandler = null;
            }
        }
        protected internal void AddFileDownload(IDownloadFile file)
        {
            lock (_queue_Download)
            {
                _queue_Download.Add(file);
            }
        }
        protected internal void AbortFileDownload(DownloadFileImplementation file)
        {
            lock (_queue_Download)
            {
                file.Status = FileStatus.CANCELED;
                file.StatusDetails = "CANCELED";
                file.OnFileDownloadCallback();
                _downloadManager.Remove(file.Id);
            }
        }
        protected internal void RemoveFileDownload(IDownloadFile file)
        {
            lock (_queue_Download)
            {
                _queue_Download.Remove(file);
                StopDownloadWatcher();
            }
        }
        public void UpdateFileDownloadProperties(ICursor cursor, DownloadFileImplementation downloadFile)
        {
            var totalBytesWritten = cursor.GetFloat(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnBytesDownloadedSoFar));
            var totalBytesExpected = cursor.GetFloat(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnTotalSizeBytes));
            downloadFile.OnFileDownloadProgress(totalBytesWritten, totalBytesExpected);
            switch ((DownloadStatus)cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnStatus)))
            {
                case DownloadStatus.Successful:
                    string pathFile = cursor.GetString(cursor.GetColumnIndex("local_uri"));
                    string type = cursor.GetString(cursor.GetColumnIndex("media_type"));
                    string path = pathFile.Replace("file://", "");
                    downloadFile.DestinationPathName = path;
                    downloadFile.MimeType = type;
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = FileStatus.COMPLETED;
                    downloadFile.OnFileDownloadCallback();
                    break;

                case DownloadStatus.Failed:
                    var reasonFailed = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnReason));
                    if (reasonFailed < 600)
                    {
                        downloadFile.StatusDetails = "Error.HttpCode: " + reasonFailed;
                    }
                    else
                    {
                        switch ((DownloadError)reasonFailed)
                        {
                            case DownloadError.CannotResume:
                                downloadFile.StatusDetails = "Error.CannotResume";
                                break;
                            case DownloadError.DeviceNotFound:
                                downloadFile.StatusDetails = "Error.DeviceNotFound";
                                break;
                            case DownloadError.FileAlreadyExists:
                                downloadFile.StatusDetails = "Error.FileAlreadyExists";
                                break;
                            case DownloadError.FileError:
                                downloadFile.StatusDetails = "Error.FileError";
                                break;
                            case DownloadError.HttpDataError:
                                downloadFile.StatusDetails = "Error.HttpDataError";
                                break;
                            case DownloadError.InsufficientSpace:
                                downloadFile.StatusDetails = "Error.InsufficientSpace";
                                break;
                            case DownloadError.TooManyRedirects:
                                downloadFile.StatusDetails = "Error.TooManyRedirects";
                                break;
                            case DownloadError.UnhandledHttpCode:
                                downloadFile.StatusDetails = "Error.UnhandledHttpCode";
                                break;
                            case DownloadError.Unknown:
                                downloadFile.StatusDetails = "Error.Unknown";
                                break;
                            default:
                                downloadFile.StatusDetails = "Error.Unregistered: " + reasonFailed;
                                break;
                        }
                    }
                    downloadFile.Status = FileStatus.FAILED;
                    downloadFile.OnFileDownloadCallback();
                    break;

                case DownloadStatus.Paused:
                    var reasonPaused = cursor.GetInt(cursor.GetColumnIndex(Android.App.DownloadManager.ColumnReason));
                    switch ((DownloadPausedReason)reasonPaused)
                    {
                        case DownloadPausedReason.QueuedForWifi:
                            downloadFile.StatusDetails = "Paused.QueuedForWifi";
                            break;
                        case DownloadPausedReason.WaitingToRetry:
                            downloadFile.StatusDetails = "Paused.WaitingToRetry";
                            break;
                        case DownloadPausedReason.WaitingForNetwork:
                            downloadFile.StatusDetails = "Paused.WaitingForNetwork";
                            break;
                        case DownloadPausedReason.Unknown:
                            downloadFile.StatusDetails = "Paused.Unknown";
                            break;
                        default:
                            downloadFile.StatusDetails = "Paused.Unregistered: " + reasonPaused;
                            break;
                    }
                    downloadFile.Status = FileStatus.PAUSED;
                    downloadFile.TotalRequestException++;
                    break;

                case DownloadStatus.Pending:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = FileStatus.PENDING;
                    break;

                case DownloadStatus.Running:
                    downloadFile.StatusDetails = default(string);
                    downloadFile.Status = FileStatus.RUNNING;
                    break;
            }
            if(downloadFile.TotalRequestException >= 7) //request exception tối đa 7s thì fail
            {
                downloadFile.Status = FileStatus.FAILED;
                downloadFile.StatusDetails = "Error.Exception";
                downloadFile.OnFileDownloadCallback();
            }
        }
        
        private string GetPathDownloadFile(string fileName)
        {
            var downloadFolder = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            string path = System.IO.Path.Combine(downloadFolder.Path, fileName);
            return ReplaceStringWhiteSpace(path, "_");
        }
        private string ReplaceStringWhiteSpace(string url, string strReplace)
        {
            return url.Replace(" ", strReplace);
        }
        private bool FileExistenceCheck(IDownloadFile i)
        {
            var downloadFile = (DownloadFileImplementation)i;
            var destinationPathName = GetPathDownloadFile(downloadFile.FileName);
            var file = new Java.IO.File(destinationPathName);
            if (file.Exists())
            {
                string mimeType = Android.Webkit.MimeTypeMap.Singleton.GetMimeTypeFromExtension(Android.Webkit.MimeTypeMap.GetFileExtensionFromUrl(destinationPathName.ToLower()));
                if (mimeType == null)
                    mimeType = "*/*";

                downloadFile.DestinationPathName = destinationPathName;
                downloadFile.MimeType = mimeType;
                downloadFile.StatusDetails = default(string);
                downloadFile.Status = FileStatus.COMPLETED;
                downloadFile.OnFileDownloadUpdateStatus();
                return true;
            }
            return false;
        }

        #endregion

        #region Upload file
        public IUploadFile CreateFileUpload(string url, string name, string path, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return new UploadFileImplementation(url, name, path, _activity, headers, parameters, boundary);
        }

        public IUploadFile CreateFileUpload(string url, string name, byte[] bytes, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return new UploadFileImplementation(url, name, bytes, _activity, headers, parameters, boundary);
        }
        public void StartUploadFile(IUploadFile file)
        {
            var fileUpload = (UploadFileImplementation)file;

            var uploads = _queue_Upload.Cast<UploadFileImplementation>();
            var item = uploads.Where(x => x.FileName == fileUpload.FileName)?.FirstOrDefault();
            if (item != null)
            {
                RemoveFileUpload(item);
            }
            else
            {
                fileUpload.StatusCode = FileStatus.INITIALIZED;

                AddFileUpload(fileUpload);
            }
        }
        protected internal void AddFileUpload(IUploadFile file)
        {
            lock (_queue_Upload)
            {
                _queue_Upload.Add(file);
                CheckQueueUploadFile();
            }
        }
        protected internal void RemoveFileUpload(IUploadFile file)
        {
            lock (_queue_Upload)
            {
                _queue_Upload.Remove(file);
                CheckQueueUploadFile();
            }
        }

        public void AbortAllUploadFile()
        {
            foreach (var item in _queue_Upload)
            {
                _queue_Upload.Remove(item);
            }
        }
        public void CheckQueueUploadFile()
        {
            var queueUpload = Queue_Upload.Cast<UploadFileImplementation>().Take(_maximumConnectionsPerQueue);
            foreach (var file in queueUpload)
            {
                if (file == null)
                    continue;

                if (file.StatusCode != FileStatus.INITIALIZED)
                    continue;

                file.StartUpload();
            }
        }
        #endregion

    }
}