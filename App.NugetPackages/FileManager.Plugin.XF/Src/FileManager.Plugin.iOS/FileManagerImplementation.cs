using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FileManager.Plugin.Abstractions;
using Foundation;
using UIKit;

namespace FileManager.Plugin
{
    public class FileManagerImplementation : IFileManager
    {
        private readonly NSUrlSession _backgroundSessionDownload;
        private int _maximumConnectionsPerQueue;
        public const string DownloadFile_NotifyKey = "";
        private const string identifierUpload = "fileUpload";
        private string identifierDownload => NSBundle.MainBundle.BundleIdentifier + ".BackgroundTransferSession";
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

        public void Init(int maximumConnectionsPerQueue)
        {
            _maximumConnectionsPerQueue = maximumConnectionsPerQueue;
        }

        public FileManagerImplementation()
        {
            _queue_Download = new List<IDownloadFile>();
            _queue_Upload = new List<IUploadFile>();

            _backgroundSessionDownload = CreateBackgroundSessionDownload();

            // Reinitialize tasks that were started before the app was terminated or suspended
            //_backgroundSession.GetTasks2((dataTasks, uploadTasks, downloadTasks) =>
            //{
            //    foreach (var download in downloadTasks)
            //    {
            //        //AddFile(new DownloadFileImplementation(task));
            //    }
            //    foreach (var upload in uploadTasks)
            //    {
            //        //AddFile(new DownloadFileImplementation(task));
            //    }
            //});
        }

        #region create session
        private NSUrlSession CreateBackgroundSessionDownload()
        {
            UrlSessionDownloadDelegate sessionDownloadDelegate = new UrlSessionDownloadDelegate();
            sessionDownloadDelegate.Controller = this;
            NSUrlSessionConfiguration sessionConfiguration = CreateSessionConfiguration(null, $"{identifierDownload}", string.Empty, false);
            sessionConfiguration.SessionSendsLaunchEvents = true;
            sessionConfiguration.Discretionary = true;
            var session = NSUrlSession.FromConfiguration(sessionConfiguration, (INSUrlSessionDownloadDelegate)sessionDownloadDelegate, NSOperationQueue.MainQueue);
            return session;
        }

        private NSUrlSession CreateBackgroundSessionUpload()
        {
            UrlSessionTaskDelegate sessionDataDelegate = new UrlSessionTaskDelegate();
            sessionDataDelegate.Controller = this;
            NSUrlSessionConfiguration sessionConfiguration = CreateSessionConfiguration(null, $"{identifierUpload}", string.Empty, true);
            var session = NSUrlSession.FromConfiguration(sessionConfiguration, (INSUrlSessionTaskDelegate)sessionDataDelegate, NSOperationQueue.MainQueue);
            return session;
        }

        private NSUrlSessionConfiguration CreateSessionConfiguration(IDictionary<string, string> headers, string identifier, string boundary, bool isUpload)
        {
            NSUrlSessionConfiguration sessionConfiguration;
            var headerDictionary = new NSMutableDictionary();
            if (isUpload)
            {
                sessionConfiguration = NSUrlSessionConfiguration.DefaultSessionConfiguration;
                headerDictionary.Add(new NSString("Accept"), new NSString("application/json"));
                headerDictionary.Add(new NSString("Content-Type"), new NSString(string.Format("multipart/form-data; boundary={0}", boundary)));
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                sessionConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(identifier);
            }
            else
            {
                sessionConfiguration = NSUrlSessionConfiguration.BackgroundSessionConfiguration(identifier);
            }
            if (headers != null && headers.Keys.Any())
            {
                foreach (string key in headers.Keys)
                {
                    if (!string.IsNullOrEmpty(headers[key]))
                    {
                        var headerKey = new NSString(key);
                        if (headerDictionary.ContainsKey(new NSString(key)))
                        {
                            headerDictionary[headerKey] = new NSString(headers[key]);
                        }
                        else
                        {
                            headerDictionary.Add(new NSString(key), new NSString(headers[key]));
                        }

                    }
                }
            }
            sessionConfiguration.HttpAdditionalHeaders = headerDictionary;
            sessionConfiguration.AllowsCellularAccess = true;

            sessionConfiguration.NetworkServiceType = NSUrlRequestNetworkServiceType.Default;
            //sessionConfiguration.TimeoutIntervalForRequest = 30;
            //sessionConfiguration.HttpMaximumConnectionsPerHost = 1;
            return sessionConfiguration;
        }

        #endregion

        #region Download

        public IDownloadFile CreateFileDownload(string url, string fileName = null)
        {
            return CreateFileDownload(url, new Dictionary<string, string>(), fileName);
        }

        public IDownloadFile CreateFileDownload(string url, IDictionary<string, string> headers, string fileName = null)
        {
            var downloads = Queue_Download.Cast<DownloadFileImplementation>();
            var fileDownload = downloads.Where(x => x.Url == url.Replace(" ", "%20")).FirstOrDefault();
            return (fileDownload != null) ? fileDownload : new DownloadFileImplementation(url, headers, fileName);
        }
        public void StartDownloadManager()
        {
            var downloads = Queue_Download.Cast<DownloadFileImplementation>().Take(_maximumConnectionsPerQueue);
            foreach (var file in downloads)
            {
                if (file == null)
                    continue;

                if (file.Status != FileStatus.INITIALIZED)
                    return;

                file.StartDownload(_backgroundSessionDownload, true);
            }
        }

        public void StartDownloadFile(IDownloadFile file)
        {
            var fileDownload = (DownloadFileImplementation)file;
            if (fileDownload.Status == FileStatus.NONE)
            {
                if (FileExistenceCheck(fileDownload))
                    return;

                fileDownload.Status = FileStatus.INITIALIZED;

                fileDownload.OnFileDownloadUpdateStatus();
                AddFileDownload(fileDownload);

                StartDownloadManager();
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

        public void AbortFileDownload(DownloadFileImplementation file)
        {
            file.Status = FileStatus.CANCELED;
            file.StatusDetails = "CANCELED";
            file.Task?.Cancel();
            file.OnFileDownloadUpdateStatus();
            RemoveFileDownload(file);
        }
        protected internal void AddFileDownload(IDownloadFile file)
        {
            lock (_queue_Download)
            {
                _queue_Download.Add(file);
            }
        }
        protected internal void RemoveFileDownload(IDownloadFile file)
        {
            lock (_queue_Download)
            {
                _queue_Download.Remove(file);
            }
            StartDownloadManager();
        }

        private bool FileExistenceCheck(IDownloadFile i)
        {
            var fileManager = NSFileManager.DefaultManager;
            var downloadFile = (DownloadFileImplementation)i;

            var destinationURL = Common.GetPathDownloadFile(fileManager, downloadFile.FileName);
            string pathFile = destinationURL.Path;

            if (fileManager.FileExists(pathFile))
            {
                downloadFile.DestinationPathName = pathFile;
                downloadFile.StatusDetails = default(string);
                downloadFile.Status = FileStatus.COMPLETED;
                downloadFile.OnFileDownloadUpdateStatus();
                return true;
            }
            return false;
        }
        #endregion

        #region Upload

        public IUploadFile CreateFileUpload(string url, string name, string path, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            if (string.IsNullOrEmpty(boundary))
            {
                boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            }
            return new UploadFileImplementation(url, name, path, headers, parameters, boundary);
        }

        public IUploadFile CreateFileUpload(string url, string name, byte[] bytes, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            if (string.IsNullOrEmpty(boundary))
            {
                boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            }
            return new UploadFileImplementation(url, name, bytes, headers, parameters, boundary);
        }
        public void StartUploadManager()
        {
            var uploads = Queue_Upload.Cast<UploadFileImplementation>().Take(_maximumConnectionsPerQueue);
            foreach (var file in uploads)
            {
                if (file == null)
                    continue;

                if (file.StatusCode != FileStatus.INITIALIZED)
                    return;

                file.StatusCode = FileStatus.RUNNING;
                var sesssion = CreateBackgroundSessionUpload();

                file.StartUpload(sesssion);
            }
        }

        public void StartUploadFile(IUploadFile file)
        {
            var uploadFile = (UploadFileImplementation)file;
            var uploads = Queue_Download.Cast<UploadFileImplementation>().ToList();
            var item = uploads.Where(x => x.Url == uploadFile.Url).FirstOrDefault();
            if (item != null)
            {
                AbortFileUpload(item);
            }
            else
            {
                uploadFile.StatusCode = FileStatus.INITIALIZED;

                AddFileUpload(uploadFile);

                StartUploadManager();
            }
        }

        public void AbortFileUpload(UploadFileImplementation file)
        {
            file.StatusCode = FileStatus.CANCELED;
            file.Task?.Cancel();
            RemoveFileUpload(file);
        }
        protected internal void AddFileUpload(IUploadFile file)
        {
            lock (_queue_Upload)
            {
                _queue_Upload.Add(file);
            }
        }
        protected internal void RemoveFileUpload(IUploadFile file)
        {
            lock (_queue_Upload)
            {
                _queue_Upload.Remove(file);
                StartUploadManager();
            }
        }
        public void AbortAllUploadFile()
        {
            foreach (var item in _queue_Upload)
            {
                _queue_Upload.Remove(item);
            }
        }
        #endregion

    }
}