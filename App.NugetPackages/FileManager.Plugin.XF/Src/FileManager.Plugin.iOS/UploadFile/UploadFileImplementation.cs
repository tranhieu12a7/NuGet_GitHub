using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreFoundation;
using FileManager.Plugin.Abstractions;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace FileManager.Plugin
{
    public class UploadFileImplementation : IUploadFile
    {
        public NSUrlSessionTask Task;
        private readonly Encoding encoding = Encoding.UTF8;
        private const string UploadFileSuffix = "-multi-part";
        public event EventHandler<FileResultStatus> FileUploadCallback;
        public event EventHandler<FileResultProgress> FileUploadProgress;
        IDictionary<nuint, NSMutableData> uploadData = new Dictionary<nuint, NSMutableData>();
        object _lock = new object();
        public string Url { get; }

        public string FileName { get; }

        public string Path { get; }

        public byte[] Bytes { get; }

        public IDictionary<string, string> Headers { get; }

        public IDictionary<string, string> Parameters { get; }

        public string Boundary { get; }

        public string DataResult { get; set; }

        public FileStatus StatusCode { get; set; }
        public string StatusDetail { get; set; }
        private string OriginalPath { get; set; }
        private bool IsTemporal { get; set; }

        public UploadFileImplementation(string url, string name, string path, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            Url = url;
            FileName = name;
            Path = path;
            Headers = headers;
            Parameters = parameters;
            Boundary = boundary;
            var tmpPath = path;
            var fileName = tmpPath.Substring(tmpPath.LastIndexOf("/") + 1);
            if (path.StartsWith("/var/"))
            {
                var data = NSData.FromUrl(new NSUrl($"file://{path}"));
                OriginalPath = Common.SaveToDisk(data, "tmp", fileName);
                IsTemporal = true;
            }
        }

        public UploadFileImplementation(string url, string name, byte[] bytes, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            Url = url;
            FileName = name;
            Bytes = bytes;
            Headers = headers;
            Parameters = parameters;
            Boundary = boundary;
        }
        public async Task<string> SaveToFileSystemAsync()
        {
            
            if (!string.IsNullOrEmpty(this.OriginalPath) && !File.Exists(this.OriginalPath))
            {
                this.StatusCode = FileStatus.FAILED;
                string error = $"File at path: {this.Path} doesn't exist";
                this.StatusDetail = error;
                this.OnFileUploadCallback();
                return string.Empty;
            }
            
            return await System.Threading.Tasks.Task.Run(() =>
            {
                lock (_lock)
                {
                    // Construct the body
                    System.Text.StringBuilder sb = new System.Text.StringBuilder("");
                    if (this.Parameters != null)
                    {
                        foreach (string vkp in this.Parameters.Keys)
                        {
                            if (this.Parameters[vkp] != null)
                            {
                                sb.AppendFormat("--{0}\r\n", this.Boundary);
                                sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", vkp);
                                sb.AppendFormat("{0}\r\n", this.Parameters[vkp]);
                            }
                        }
                    }

                    string tmpPath = Common.GetOutputPath("tmp", "tmp", this.FileName);
                    var multiPartPath = $"{tmpPath}{DateTime.Now.ToString("yyyMMdd_HHmmss")}{UploadFileSuffix}";

                    // Delete any previous body data file
                    if (File.Exists(multiPartPath))
                        File.Delete(multiPartPath);


                    using (var writeStream = new FileStream(multiPartPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        writeStream.Write(encoding.GetBytes(sb.ToString()), 0, encoding.GetByteCount(sb.ToString()));

                        sb.Clear();
                        sb.AppendFormat("--{0}\r\n", this.Boundary);
                        sb.Append($"Content-Disposition: form-data; name=\"{nameof(this.FileName)}\"; filename=\"{this.FileName}\"\r\n");
                        sb.Append($"Content-Type: {Common.GetMimeType(this.FileName)}\r\n\r\n");

                        writeStream.Write(encoding.GetBytes(sb.ToString()), 0, encoding.GetByteCount(sb.ToString()));
                        if (this.Bytes != null)
                        {
                            writeStream.Write(this.Bytes, 0, this.Bytes.Length);
                        }
                        else if (!string.IsNullOrEmpty(this.OriginalPath) && File.Exists(this.OriginalPath))
                        {
                            var data = File.ReadAllBytes(this.OriginalPath);
                            writeStream.Write(data, 0, data.Length);
                        }

                        writeStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                        var pBoundary = $"\r\n--{this.Boundary}--\r\n";
                        writeStream.Write(encoding.GetBytes(pBoundary), 0, encoding.GetByteCount(pBoundary));

                        //delete temporal files created
                        if (IsTemporal && File.Exists(this.OriginalPath))
                        {
                            File.Delete(this.OriginalPath);
                        }
                    }

                    sb = null;
                    return multiPartPath;
                }
            });
        }
        public async void StartUpload(NSUrlSession session)
        {
            var mPath = await SaveToFileSystemAsync();
            if (string.IsNullOrEmpty(mPath))
                return;

            NSUrl nSUrlFile = new NSUrl(mPath, false);

            var request = new NSMutableUrlRequest(NSUrl.FromString(this.Url));
            request.HttpMethod = "POST";
            request.BodyStream = new NSInputStream(nSUrlFile);
            request["Accept"] = "*/*";
            request["Content-Type"] = "multipart/form-data; boundary=" + this.Boundary;
            request.AllowsCellularAccess = true;
            //request.SetValueForKey(new NSString("Accept"), new NSString("*/*"));
            //request.SetValueForKey(new NSString("Content-Type"), new NSString(string.Format("multipart/form-data; boundary={0}", this.Boundary)));

            //var session2 = NSUrlSession.FromConfiguration(NSUrlSessionConfiguration.DefaultSessionConfiguration , (INSUrlSessionTaskDelegate)this, NSOperationQueue.MainQueue);
            Task = session.CreateUploadTask(request, nSUrlFile, (data,response,error) => {
                if(data != null)
                {
                    this.DataResult = data.ToString();
                }
                this.OnFileUploadCallback();
            });

            Task.TaskDescription = mPath;
            Task.Priority = NSUrlSessionTaskPriority.High;
            Task.Resume();
        }
        public void OnFileUploadProgress(float totalBytes, float totalLength)
        {
            NSOperationQueue.MainQueue.InvokeOnMainThread(() =>
            {
                var fileUploadProgress = new FileResultProgress(totalBytes, totalLength);
                FileUploadProgress(this, fileUploadProgress);
            });

        }

        public void OnFileUploadCallback()
        {
            NSOperationQueue.MainQueue.InvokeOnMainThread(() =>
            {
                var fileUploadResponse = new FileResultStatus(this.StatusDetail, this.StatusCode);
                FileUploadCallback(this, fileUploadResponse);
                (CrossFileManager.Current as FileManagerImplementation).RemoveFileUpload(this);
            });
        }

        //public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        //{
        //    Console.WriteLine(string.Format("DidCompleteWithError TaskId: {0}{1}", task.TaskIdentifier, (error == null ? "" : " Error: " + error.Description)));
        //    NSMutableData _data = null;

        //    if (uploadData.ContainsKey(task.TaskIdentifier))
        //    {
        //        _data = uploadData[task.TaskIdentifier];
        //        uploadData.Remove(task.TaskIdentifier);
        //    }
        //    else
        //    {
        //        _data = new NSMutableData();
        //    }

        //    NSString dataString = NSString.FromData(_data, NSStringEncoding.UTF8);
        //    var message = dataString == null ? string.Empty : $"{dataString}";
        //    var responseError = false;
        //    NSHttpUrlResponse response = null;

        //    string parts = task.TaskDescription;

        //    if (task.Response is NSHttpUrlResponse)
        //    {
        //        response = task.Response as NSHttpUrlResponse;
        //        Console.WriteLine("HTTP Response {0}", response);
        //        Console.WriteLine("HTTP Status {0}", response.StatusCode);
        //        responseError = response.StatusCode != 200 && response.StatusCode != 201;
        //    }

        //    System.Diagnostics.Debug.WriteLine("COMPLETE");

        //    //Remove the temporal multipart file
        //    if (!string.IsNullOrEmpty(parts) && File.Exists(parts))
        //    {
        //        File.Delete(parts);
        //    }

        //    if (error == null && !responseError)
        //    {
        //        this.Status = FileStatus.FAILED;
        //        this.OnFileUploadCallback(message);
        //    }
        //    else if (responseError)
        //    {
        //        this.Status = FileStatus.FAILED;
        //        this.OnFileUploadCallback(message);
        //    }
        //    else
        //    {
        //        this.Status = FileStatus.FAILED;
        //        this.OnFileUploadCallback(error.Description);
        //    }

        //    _data = null;
        //}
        //public override void DidFinishCollectingMetrics(NSUrlSession session, NSUrlSessionTask task, NSUrlSessionTaskMetrics metrics)
        //{
        //    this.Status = FileStatus.COMPLETED;
        //    this.OnFileUploadCallback(string.Empty);
        //}

        //public override void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidReceiveData...");
        //    if (uploadData.ContainsKey(dataTask.TaskIdentifier))
        //    {
        //        uploadData[dataTask.TaskIdentifier].AppendData(data);
        //    }
        //    else
        //    {
        //        var uData = new NSMutableData();
        //        uData.AppendData(data);
        //        uploadData.Add(dataTask.TaskIdentifier, uData);
        //    }
        //    // _data.AppendData(data);
        //}

        //public override void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidReceiveResponse:  " + response.ToString());

        //    completionHandler.Invoke(NSUrlSessionResponseDisposition.Allow);
        //}

        //public override void DidBecomeDownloadTask(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionDownloadTask downloadTask)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidBecomeDownloadTask");
        //}


        //public override void DidBecomeInvalid(NSUrlSession session, NSError error)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidBecomeInvalid" + (error == null ? "undefined" : error.Description));
        //}


        //public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
        //{
        //    System.Diagnostics.Debug.WriteLine("DidFinishEventsForBackgroundSession");

        //    var completionHandler = CrossFileManager.BackgroundSessionCompletionHandler;
        //    if (completionHandler != null)
        //    {
        //        CrossFileManager.BackgroundSessionCompletionHandler = null;

        //        DispatchQueue.MainQueue.DispatchAsync(() =>
        //        {
        //            completionHandler();
        //        });
        //    }
        //}

        //public override void DidSendBodyData(NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
        //{

        //    this.OnFileUploadProgress(totalBytesSent, totalBytesExpectedToSend);

        //    System.Diagnostics.Debug.WriteLine(string.Format("DidSendBodyData bSent: {0}, totalBSent: {1} totalExpectedToSend: {2}", bytesSent, totalBytesSent, totalBytesExpectedToSend));
        //}
    }
}