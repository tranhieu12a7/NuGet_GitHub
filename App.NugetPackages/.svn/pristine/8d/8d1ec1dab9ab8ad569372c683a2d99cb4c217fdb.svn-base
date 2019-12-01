using System;
using System.Collections.Generic;

namespace FileManager.Plugin.Abstractions
{
    public interface IFileManager
    {
        /// <summary>
        /// lấy hàng đợi của download
        /// </summary>
        IEnumerable<IDownloadFile> Queue_Download { get; }
        /// <summary>
        /// lấy hàng đợi của upload
        /// </summary>
        IEnumerable<IUploadFile> Queue_Upload { get; }
        /// <summary>
        /// tạo file download để nhận được result và progress
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IDownloadFile CreateFileDownload(string url, string fileName = null);
        /// <summary>
        /// tạo file download để nhận được result và progress
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IDownloadFile CreateFileDownload(string url, IDictionary<string, string> headers, string fileName = null);
        /// <summary>
        /// bắt đầu download
        /// </summary>
        /// <param name="file"></param>
        void StartDownloadFile(IDownloadFile file);
        void AbortDownloadFile(IDownloadFile file);
        /// <summary>
        /// tạo file upload để nhận được result và progress
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IUploadFile CreateFileUpload(string url, string name, string path, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);
        /// <summary>
        /// tạo file upload để nhận được result và progress
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        IUploadFile CreateFileUpload(string url, string name, byte[] bytes, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);
        /// <summary>
        /// bắt đầu upload
        /// </summary>
        /// <param name="file"></param>
        void StartUploadFile(IUploadFile file);
        void AbortAllUploadFile();
    }
}
