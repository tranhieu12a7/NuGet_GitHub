using FileManager.Plugin;
using FileManager.Plugin.Abstractions;
using Plugin.FilePicker;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SampleFileManager
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        string pathFile = string.Empty;
        string pathFile2 = string.Empty;
        //const string url = "http://192.168.1.86:8001/WebSite/upload.ashx";
        //const string url = "http://192.168.1.86:8001/ServiceAPI/api/uploadFiles";
        const string url = "http://192.168.1.24:8080/alfresco/service/api/upload";
        //const string url = "http://192.168.1.78:8123/api/UploadFileAlfresco";
        public MainPage()
        {
            InitializeComponent();
        }
        private void Button_Clicked(object sender, EventArgs e)
        {
            //string url = "https://hocmon-online.tphcm.gov.vn/App.HocMon/website//uploads/App/Document/2019/07/Master-MFC_20190731143118825.pdf";
            string url2 = @"http://demo.vietinfo.tech:9998/App.HocMon/website/uploads/App/Document/2019/08/BC tham luan - Hoi nghi quan tri mang Quan 12_20190829110608314.pptx";
            //string url2 = "http://192.168.1.86:8001/Website/uploads/VBCD/5439_BYT-TCCB_m_317151.doc";

            //string url2 = "http://demo.vietinfo.tech:9998/App.HocMon/website/uploads/App/20MB_2.zip";

            //string pathFileServer = "/Uploads/Documents/5439_BYT-TCCB_m_317151.doc";
            string pathFileServer = "/Uploads/Documents/123123.zip";
            //string url2 = "http://192.168.1.86:8001/ServiceAPI/api/DownloadFiles?fileName=" + pathFileServer;
            //var file = CrossFileManager.Current.CreateFileDownload(url);
            var file2 = CrossFileManager.Current.CreateFileDownload(url2);

            //file.FileDownloadCallback += (filedata, obj) =>
            //{
            //    var data = filedata as IDownloadFile;
            //    var data2 = obj as FileResultStatus;

            //    pathFile = data.DestinationPathName;
            //    Console.WriteLine("FileDownloadCallback:" + data2.StatusCode);
            //};

            //file.FileDownloadProgress += (eve, obj) =>
            //{
            //    var data = obj as FileResultProgress;
            //    if (data == null) return;

            //    lblProgress1.Text = $"Progress: {data.Percentage}";
            //    Console.WriteLine("FileDownloadProgress:" + data.Percentage);
            //};

            file2.FileDownloadCallback += (filedata, obj) =>
            {
                var data = filedata as IDownloadFile;
                var data2 = obj as FileResultStatus;

                pathFile2 = data.DestinationPathName;
                Console.WriteLine("FileDownloadCallback_2:" + data2.StatusCode);
            };

            file2.FileDownloadProgress += (eve, obj) =>
            {
                var data = obj as FileResultProgress;
                if (data == null) return;

                lblProgress2.Text = $"Progress_2: {data.Percentage}";
                Console.WriteLine("FileDownloadProgress_2:" + data.Percentage);
            };

            //CrossFileManager.Current.StartDownloadFile(file);
            CrossFileManager.Current.StartDownloadFile(file2);
        }
        private void Button_Clicked_1(object sender, EventArgs e)
        {
            //string pathFile = "/storage/emulated/0/Download/5439_BYT-TCCB_m_317151.doc";
            string pathFile = "/storage/emulated/0/Download/BC_tham_luan_-_Hoi_nghi_quan_tri_mang_Quan_12_20190829110608314.pptx";
            string pathFile2 = "/storage/emulated/0/Download/10MB.zip";

            if (string.IsNullOrEmpty(pathFile) || string.IsNullOrEmpty(pathFile2))
            {
                Console.WriteLine("Click Download File và đợi down xong");
            }

            int count_1 = 0;
            int count_2 = 0;

            string name = pathFile.Split('/').Last();
            string name2 = pathFile2.Split('/').Last();

            IDictionary<string,string> keyValues = new Dictionary<string, string>();
            keyValues.Add("User", "MTL");
            var file = CrossFileManager.Current.CreateFileUpload(url, name, pathFile,null, keyValues);
            //var file2 = CrossFileManager.Current.CreateFileUpload(url, name2, pathFile2);
            file.FileUploadCallback += (eve, obj) =>
            {
                var xxx = eve as IUploadFile;
                var data = obj as FileResultStatus;
                count_1++;
                lblProgress1.Text = data.StatusCode.ToString();
                Console.WriteLine("FileUploadCallback_1:" + data.StatusCode + $"({count_1})");
            };

            file.FileUploadProgress += (eve, obj) =>
            {
                var data = obj as FileResultProgress;
                if (data == null) return;
                lblProgress2.Text = data.Percentage.ToString();
                Console.WriteLine("FileUploadProgress_1:" + data.Percentage);
            };
            //file2.FileUploadCallback += (eve, obj) =>
            //{
            //    var xxx = eve as IDownloadFile;
            //    var data = obj as FileResultStatus;
            //    count_2++;
            //    Console.WriteLine("FileUploadCallback_2:" + data.StatusCode + $"({count_2})");
            //};

            //file2.FileUploadProgress += (eve, obj) =>
            //{
            //    var data = obj as FileResultProgress;
            //    if (data == null) return;
            //    Console.WriteLine("FileUploadProgress_2:" + data.Percentage);
            //};

            CrossFileManager.Current.StartUploadFile(file);
            //CrossFileManager.Current.StartUploadFile(file2);
        }

        private async void PickFile_Clicked(object sender, EventArgs e)
        {
            try
            {
                var file = await CrossFilePicker.Current.PickFile();

                if (file == null)
                    return;

                int count_1 = 0;
                IDictionary<string, string> keyValues = new Dictionary<string, string>();
                keyValues.Add("User", "MTL");
                var fileUpload = CrossFileManager.Current.CreateFileUpload(url, file.FileName, file.DataArray, null, keyValues);
                fileUpload.FileUploadCallback += (eve, obj) =>
                {
                    var xxx = eve as IDownloadFile;
                    var data = obj as FileResultStatus;
                    count_1++;
                    Console.WriteLine("FileUploadCallback_1:" + data.StatusCode + $"({count_1})");
                };

                fileUpload.FileUploadProgress += (eve, obj) =>
                {
                    var data = obj as FileResultProgress;
                    if (data == null) return;
                    lblProgress2.Text = $"Progress: {data.Percentage.ToString()}";
                    //Console.WriteLine("FileUploadProgress_1:" + data.Percentage);
                };
                CrossFileManager.Current.StartUploadFile(fileUpload);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private async void PickVideo_Clicked(object sender, EventArgs e)
        {
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                return;
            }

            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreVideoOptions());
            int count_1 = 0;
            if (file == null)
                return;

            var filePath = file.Path;
            IDictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("User", "MTL");
            var fileUpload = CrossFileManager.Current.CreateFileUpload(url, file.Path.Split('/').Last(),filePath,null, keyValues);
            fileUpload.FileUploadCallback += (eve, obj) =>
            {
                var xxx = eve as IUploadFile;
                var data = obj as FileResultStatus;
                count_1++;
                lblData.Text = xxx.DataResult;
                Console.WriteLine("FileUploadCallback_1:" + data.StatusCode + $"({count_1})");
            };

            fileUpload.FileUploadProgress += (eve, obj) =>
            {
                var data = obj as FileResultProgress;
                if (data == null) return;
                lblProgress1.Text = $"Progress: {data.Percentage}";
                Console.WriteLine("FileUploadProgress_1:" + data.Percentage);
            };
            CrossFileManager.Current.StartUploadFile(fileUpload);

            file.Dispose();

        }
    }
}
