using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FileManager.Plugin.Abstractions;
using Foundation;
using MobileCoreServices;
using UIKit;

namespace FileManager.Plugin
{
    public class Common
    {
        public static NSUrl GetPathDownloadFile(NSFileManager fileManager, string fileName)
        {
            var URLs = fileManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User);
            NSUrl documentsDictionry = URLs.First();
            NSUrl destinationURL = documentsDictionry.Append(fileName, false);
            return destinationURL;
        }
        public static string GetOutputPath(string directoryName, string bundleName, string name)
        {
#if __MAC__
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directoryName);
#else
            var path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), directoryName);
#endif
            Directory.CreateDirectory(path);

            if (string.IsNullOrWhiteSpace(name))
            {
                string timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");

                name = $"{bundleName}_{timestamp}.jpg";
            }


            return System.IO.Path.Combine(path, GetUniquePath(path, name));
        }

        public static string GetUniquePath(string path, string name)
        {

            string ext = System.IO.Path.GetExtension(name);
            if (ext == String.Empty)
                ext = ".jpg";

            name = System.IO.Path.GetFileNameWithoutExtension(name);

            string nname = name + ext;
            int i = 1;
            while (File.Exists(System.IO.Path.Combine(path, nname)))
                nname = name + "_" + (i++) + ext;


            return System.IO.Path.Combine(path, nname);
        }

        public static string GetMimeType(string fileName)
        {
#if __MAC__
            try
            {
                var extensionWithDot = Path.GetExtension(fileName);
                if (!string.IsNullOrWhiteSpace(extensionWithDot))
                {
                    var extension = extensionWithDot.Substring(1);
                    if (!string.IsNullOrWhiteSpace(extension)&&mimeTypes.ContainsKey(extension))
                    {
                       return mimeTypes[extension];
                    }
                }
            }catch(Exception ex)
            {

            }
#else
            try
            {
                var extensionWithDot = System.IO.Path.GetExtension(fileName);
                if (!string.IsNullOrWhiteSpace(extensionWithDot))
                {
                    var extension = extensionWithDot.Substring(1);
                    if (!string.IsNullOrWhiteSpace(extension))
                    {
                        var extensionClassRef = new NSString(UTType.TagClassFilenameExtension);
                        var mimeTypeClassRef = new NSString(UTType.TagClassMIMEType);

                        var uti = NativeTools.UTTypeCreatePreferredIdentifierForTag(extensionClassRef.Handle, new NSString(extension).Handle, IntPtr.Zero);
                        var mimeType = NativeTools.UTTypeCopyPreferredTagWithClass(uti, mimeTypeClassRef.Handle);
                        using (var mimeTypeCString = new CoreFoundation.CFString(mimeType))
                        {
                            return mimeTypeCString;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
#endif
            return "*/*";
        }

        public static string SaveToDisk(NSData data, string bundleName, string fileName = null, string directoryName = null)
        {


            NSError err = null;
            string path = GetOutputPath(directoryName ?? bundleName, bundleName, fileName);

            if (!File.Exists(path))
            {

                if (data.Save(path, true, out err))
                {
                    System.Diagnostics.Debug.WriteLine("saved as " + path);
                    Console.WriteLine("saved as " + path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("NOT saved as " + path +
                        " because" + err.LocalizedDescription);
                }

            }

            return path;
        }
    }
}