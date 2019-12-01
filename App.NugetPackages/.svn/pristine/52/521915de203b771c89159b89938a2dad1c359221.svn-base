
Lưu ý: có thay đổi về code, hướng dẫn ở phần cách dùng
#### Cau hinh

**Android**

_Add them OkHttp.dll ( lien he MTL de lay dll )

_Trong tap tin AndroidManifest.xml them quyen:

<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />

_Trong tap tin MainActivity.cs them init o trong ham OnCreate():

// 2 la so hang doi
(CrossFileManager.Current as FileManagerImplementation).Init(this,2); 

**iOS**

_Trong tap tin Info.plist them quyen:
<key>UIBackgroundModes</key>
<array>
	<string>fetch</string>
</array>

_Trong tap tin AppDelegate.cs them:
// 2 la so hang doi, nam trong ham FinishedLaunching.
(CrossFileManager.Current as FileManagerImplementation).Init(2);

public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, System.Action completionHandler)
{
    CrossFileManager.BackgroundSessionCompletionHandler = completionHandler;
}

#### Cach dung ( co the gui dataArray or pathFile )

**Upload file**

var fileUpload = CrossFileManager.Current.CreateFileUpload(/* url */, /* ten file */, /* duong dan */);
fileUpload.FileUploadCallback += (obj, result) =>
{
    IUploadFile downloadFile = obj as IUploadFile;
    FileResultStatus fileResult = result as FileResultStatus;
    switch (fileResult.StatusCode)
    {
        case FileStatus.COMPLETED:
			//downloadFile.DataResult la upload tra ve link da upload
            break;
        case FileStatus.FAILED:
            break;
    }
};

fileUpload.FileUploadProgress += (obj, result)) =>
{
	IUploadFile downloadFile = obj as IUploadFile;
    FileResultProgress fileResult = result as FileResultProgress;
	//fileResult.Percentage la so %
};
CrossFileManager.Current.StartUploadFile(fileUpload);

**Download File**

var file = CrossFileManager.Current.CreateFileDownload(/* url */);
file.FileDownloadCallback += (obj, result) =>
{
    IDownloadFile downloadFile = obj as IDownloadFile;
    FileResultStatus fileResult = result as FileResultStatus;
    switch (fileResult.StatusCode)
    {
		case FileStatus.INITIALIZED:
		case FileStatus.RUNNING:
			IsLoading = true;
			break;
        case FileStatus.COMPLETED:
			IsDownloaded = true;
			//downloadFile.DestinationPathName // duong dan download vao
            break;
		case FileStatus.CANCELED:
			IsLoading = false;
			break;
        case FileStatus.FAILED:
            break;
    }
};

file.FileDownloadProgress += (obj, result) =>
{
	IDownloadFile downloadFile = obj as IDownloadFile;
    FileResultStatus fileResult = result as FileResultStatus;
   //fileResult.Percentage la so %
};

CrossFileManager.Current.StartDownloadFile(file);//bắt đầu download
CrossFileManager.Current.StartDownloadFile(file);//hủy bỏ download