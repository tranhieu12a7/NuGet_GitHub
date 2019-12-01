using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using FileManager.Plugin.Abstractions;
using Java.Util.Concurrent;
using OkHttp;

namespace FileManager.Plugin
{
    public class UploadFileImplementation : IUploadFile, ICountProgressListener
    {
        private int CountCallback = 0;
        private TimeUnit UploadTimeoutUnit = TimeUnit.Seconds;
        public static long SocketUploadTimeout { get; set; } = 10;
        public static long ConnectUploadTimeout { get; set; } = 10;
        private object _lock = new object();

        public event EventHandler<FileResultStatus> FileUploadCallback;
        public event EventHandler<FileResultProgress> FileUploadProgress;
        public string Url { get; }

        public string FileName { get; }

        public string Path { get; }

        public byte[] Bytes { get; }
        public IDictionary<string, string> Headers { get; }

        public IDictionary<string, string> Parameters { get; }

        public string Boundary { get; }

        public FileStatus StatusCode { get; set; }
        
        private int TotalRequestException { get; set; }

        public string DataResult { get; set; }
        private Activity activity;
        TaskCompletionSource<FileResultStatus> uploadCompletionSource;
        public UploadFileImplementation(string url, string name, string path, Activity activity, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            Url = url;
            FileName = name;
            Path = path;
            Headers = headers;
            Parameters = parameters;
            Boundary = boundary;
            this.activity = activity;
        }

        public UploadFileImplementation(string url, string name, byte[] bytes, Activity activity, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            Url = url;
            FileName = name;
            Bytes = bytes;
            Headers = headers;
            Parameters = parameters;
            Boundary = boundary;
            this.activity = activity;
        }

        public async void StartUpload()
        {
            uploadCompletionSource = new TaskCompletionSource<FileResultStatus>();

            _ = Task.Run(() =>
              {
                  try
                  {
                      var requestBodyBuilder = GetRequestBodyBuilder();

                      MakeRequest(requestBodyBuilder);
                  }
                  catch (Java.Net.UnknownHostException ex)
                  {
                      this.StatusCode = FileStatus.FAILED;
                      this.OnFileUploadCallback("Host not reachable");
                  }
                  catch (Java.IO.IOException ex)
                  {
                      this.StatusCode = FileStatus.FAILED;
                      this.OnFileUploadCallback(ex.ToString());
                  }
                  catch (Exception ex)
                  {
                      this.StatusCode = FileStatus.FAILED;
                      this.OnFileUploadCallback(ex.ToString());
                  }
              });

            _ = await uploadCompletionSource.Task;
        }
        private MultipartBuilder GetRequestBodyBuilder()
        {
            var requestBodyBuilder = PrepareRequest();

            var mediaType = MediaType.Parse(GetMimeType(this.FileName));

            if (mediaType == null)
                mediaType = MediaType.Parse("*/*");

            RequestBody fileBody = null;
            if (this.Bytes != null)
            {
                fileBody = RequestBody.Create(mediaType, this.Bytes);
            }
            else
            {
                Java.IO.File _file = new Java.IO.File(this.Path);
                fileBody = RequestBody.Create(MediaType.Parse(GetMimeType(this.Path)), _file);
            }

            requestBodyBuilder.AddFormDataPart("filedata", this.FileName, fileBody);
            //requestBodyBuilder.AddFormDataPart("WWW-Authenticate", "Basic realm=Alfresco");
            return requestBodyBuilder;
        }
        private MultipartBuilder PrepareRequest()
        {
            MultipartBuilder requestBodyBuilder = null;

            if (string.IsNullOrEmpty(this.Boundary))
            {
                requestBodyBuilder = new MultipartBuilder()
                        .Type(MultipartBuilder.Form);
            }
            else
            {
                requestBodyBuilder = new MultipartBuilder(this.Boundary)
                        .Type(MultipartBuilder.Form);
            }

            if (this.Parameters != null)
            {
                foreach (string key in this.Parameters.Keys)
                {
                    if (this.Parameters[key] != null)
                    {
                        requestBodyBuilder.AddFormDataPart(key, this.Parameters[key]);
                    }
                }
            }
            return requestBodyBuilder;
        }

        public void MakeRequest(MultipartBuilder multipartBuilder)
        {
            this.StatusCode = FileStatus.RUNNING;
            CountingRequestBody requestBody = new CountingRequestBody(multipartBuilder.Build(), this);
            var requestBuilder = new Request.Builder();

            var header = this.Headers;
            if (header != null)
            {
                foreach (string key in header.Keys)
                {
                    if (!string.IsNullOrEmpty(header[key]))
                    {
                        requestBuilder = requestBuilder.AddHeader(key, header[key]);
                    }
                }
            }

            Request request = requestBuilder
                .Url(Url)
                .Post(requestBody)
                .Build();

            OkHttpClient client = new OkHttpClient();
            client.SetConnectTimeout(ConnectUploadTimeout, UploadTimeoutUnit); // connect timeout
            client.SetReadTimeout(SocketUploadTimeout, UploadTimeoutUnit);    // socket timeout
            client.SetWriteTimeout(5, UploadTimeoutUnit);
            Response response = client.NewCall(request).Execute();
            var responseString = response.Body().String();
            var code = response.Code();

            IDictionary<string, string> responseHeaders = new Dictionary<string, string>();
            var rHeaders = response.Headers();
            if (rHeaders != null)
            {
                var names = rHeaders.Names();
                foreach (string name in names)
                {
                    if (!string.IsNullOrEmpty(rHeaders.Get(name)))
                    {
                        responseHeaders.Add(name, rHeaders.Get(name));
                    }
                }
            }
            string errorStr = string.Empty;
            if (response.IsSuccessful)
            {
                this.DataResult = responseString;
                this.StatusCode = FileStatus.COMPLETED;
            }
            else
            {
                this.StatusCode = FileStatus.FAILED;
                errorStr = "Error: " + code;
            }
            this.OnFileUploadCallback(errorStr);
        }
        public void OnFileUploadProgress(long bytesWritten, long contentLength)
        {
            if (StatusCode == FileStatus.FAILED)
                return;

            this.activity.RunOnUiThread(() =>
            {
                var fileUploadProgress = new FileResultProgress(bytesWritten, contentLength);
                FileUploadProgress(this, fileUploadProgress);
            });
        }
        public void OnFileUploadCallback(string error)
        {
            lock (_lock)
            {
                if (this.StatusCode != FileStatus.FAILED && StatusCode != FileStatus.COMPLETED)
                    return;

                CountCallback++;
                if (CountCallback >= 2)
                    return;

                var fileUploadResponse = new FileResultStatus(error, this.StatusCode);

                this.activity.RunOnUiThread(() =>
                {
                    FileUploadCallback(this, fileUploadResponse);
                });

                uploadCompletionSource.TrySetResult(fileUploadResponse);

                (CrossFileManager.Current as FileManagerImplementation).RemoveFileUpload(this);
            }
        }
        public void OnFileUploadError(string error)
        {
            this.StatusCode = FileStatus.FAILED;
            OnFileUploadCallback(error);
        }
        private string GetMimeType(string url)
        {
            string type = "*/*";
            try
            {
                string extension = MimeTypeMap.GetFileExtensionFromUrl(url);
                if (!string.IsNullOrEmpty(extension))
                {
                    type = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.ToLower());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            return type;
        }

        
    }
}